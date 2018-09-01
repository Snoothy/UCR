using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Utilities;
using HidWizards.UCR.Views;
using HidWizards.UCR.ViewModels.DeviceViewModels;
using Application = System.Windows.Application;

namespace HidWizards.UCR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private static Context context;
        private HidGuardianClient _hidGuardianClient;
        private SingleGlobalInstance mutex;

        // define the systemtray icon and the contextmenu
        public System.Windows.Forms.NotifyIcon notify;

        public System.Windows.Forms.ContextMenu stMenu = new System.Windows.Forms.ContextMenu();

        protected override void OnStartup(StartupEventArgs e)
        {
            // create the systray icon
            this.notify = new System.Windows.Forms.NotifyIcon();
            this.notify.Text = "Universal Control Remapper";
            this.notify.Icon = UCR.Properties.Resources.UCR_load;
            this.notify.Visible = true;

            // minimize/restore on double click
            this.notify.DoubleClick +=
            (object sender, EventArgs args) =>
            {
                if (MainWindow.WindowState == WindowState.Normal)
                {
                    hideMainWindow();
                }
                else
                {
                    this.MainWindow.Show();
                    this.MainWindow.WindowState = WindowState.Normal;
                }
            };

            systrayContextMenu();

            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += AppDomain_CurrentDomain_UnhandledException;

            mutex = new SingleGlobalInstance();
            if (mutex.HasHandle && GetProcesses().Length <= 1)
            {
                Logger.Info("Launching UCR");
                new ResourceLoader().Load();
                _hidGuardianClient = new HidGuardianClient();
                _hidGuardianClient.WhitelistProcess();
                context = Context.Load();
                context.ParseCommandLineArguments(e.Args);
                var mw = new MainWindow(context);
                mw.Show();
            }
            else
            {
                SendArgs(string.Join(";", e.Args));
                Current.Shutdown();
            }
        }

        #region SystemTray_ContextMenu

        private void systrayContextMenu()
        {
            // Show
            var mnuShow = new System.Windows.Forms.MenuItem("Show UCR");
            stMenu.MenuItems.Add(mnuShow);
            notify.ContextMenu = stMenu;
            mnuShow.Visible = true;
            mnuShow.Click += MnuShow_Click;

            // Hide
            var mnuHide = new System.Windows.Forms.MenuItem("Hide UCR");
            stMenu.MenuItems.Add(mnuHide);
            notify.ContextMenu = stMenu;
            mnuHide.Visible = true;
            mnuHide.Click += MnuHide_Click;

            // Options
            var mnuOptions = new System.Windows.Forms.MenuItem("Options");
            stMenu.MenuItems.Add(mnuOptions);
            notify.ContextMenu = stMenu;
            mnuOptions.Visible = false;
            mnuOptions.Click += MnuOptions_Click;

            // Exit
            var mnuExit = new System.Windows.Forms.MenuItem("Exit");
            stMenu.MenuItems.Add(mnuExit);
            notify.ContextMenu = stMenu;
            mnuExit.Visible = true;
            mnuExit.Click += MnuExit_Click;
        }

        private static void MnuExit_Click(object sender, EventArgs e)
        {
            Current.Shutdown();
        }

        private static void MnuOptions_Click(object sender, EventArgs e)
        {
            // open a setup form, with a shortcut to all the configuration:
            // device groups, profiles, etc...
            openOptionsWindow();
        }

        private void MnuHide_Click(object sender, EventArgs e)
        {
            hideMainWindow();
        }

        private void MnuShow_Click(object sender, EventArgs e)
        {
            showMainWindow();
        }

        private static void openOptionsWindow()
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.Show();
        }

        private void hideMainWindow()
        {
            this.MainWindow.Hide();
            this.MainWindow.WindowState = WindowState.Minimized;
        }

        private void showMainWindow()
        {
            this.MainWindow.Show();
            this.MainWindow.WindowState = WindowState.Normal;
        }

        #endregion SystemTray_ContextMenu

        private static Process[] GetProcesses()
        {
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        }

        private static void SendArgs(string args)
        {
            Logger.Info($"UCR is already running, sending args: {{{args}}}");
            // Find the window with the name of the main form
            var processes = GetProcesses();
            processes = processes.Where(p => p.Id != Process.GetCurrentProcess().Id).ToArray();
            if (processes.Length == 0) return;

            var ptrCopyData = IntPtr.Zero;
            try
            {
                // Create the data structure and fill with data
                var copyData = new NativeMethods.COPYDATASTRUCT
                {
                    dwData = new IntPtr(2),
                    cbData = args.Length + 1,
                    lpData = Marshal.StringToHGlobalAnsi(args)
                };
                // Just a number to identify the data type
                // One extra byte for the \0 character

                // Allocate memory for the data and copy
                ptrCopyData = Marshal.AllocCoTaskMem(Marshal.SizeOf(copyData));
                Marshal.StructureToPtr(copyData, ptrCopyData, false);

                // Send the message
                foreach (var proc in processes)
                {
                    if (proc.MainWindowHandle == IntPtr.Zero) continue;
                    NativeMethods.SendMessage(proc.MainWindowHandle, NativeMethods.WM_COPYDATA, IntPtr.Zero, ptrCopyData);
                }
            }
            catch (Exception e)
            {
                Logger.Error("Unable to send args to existing process", e);
            }
            finally
            {
                // Free the allocated memory after the control has been returned
                if (ptrCopyData != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(ptrCopyData);
            }
        }

        public void Dispose()
        {
            mutex.Dispose();
            context?.Dispose();
            _hidGuardianClient?.Dispose();
            GC.SuppressFinalize(this);
            stMenu.Dispose();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Dispose();

            // clear the systray icon
            if (this.notify != null)
            {
                this.notify.Dispose();
            }
        }

        private static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception)e.ExceptionObject;
            Logger.Fatal(exception.Message, exception);
        }
    }
}