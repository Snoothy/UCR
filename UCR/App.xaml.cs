using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Utilities;
using HidWizards.UCR.Views;
using Application = System.Windows.Application;

namespace HidWizards.UCR
{
    /// <inheritdoc />
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
    public partial class App : Application, IDisposable
    {
        private Context _context;
        private HidGuardianClient _hidGuardianClient;
        private SingleGlobalInstance _mutex;

		
		/// <summary>
		/// access the notify icon assembly
		/// </summary>
		private System.Windows.Forms.NotifyIcon _notify;
		
		public System.Windows.Forms.ContextMenu TrayContextMenu = new System.Windows.Forms.ContextMenu();

        protected override void OnStartup(StartupEventArgs e)
        {
			// create the tray icon
			this._notify = new System.Windows.Forms.NotifyIcon
			{
				Text = @"Universal Control Remapper", Icon = UCR.Properties.Resources.icon_x32, Visible = true
			};

			// show and hide on tray icon doubleclick
			this._notify.DoubleClick +=
				(object sender, EventArgs args) =>
				{
					if (MainWindow != null && MainWindow.WindowState != WindowState.Normal)
					{
						this.MainWindow.Show();
						this.MainWindow.WindowState = WindowState.Normal;
					}
					else
					{
						var mainWindow = this.MainWindow;
						if (mainWindow == null) return;
						mainWindow.Hide();
						mainWindow.WindowState = WindowState.Minimized;
					}
				};

			CreateMenuStructure();

			base.OnStartup(e);
			AppDomain.CurrentDomain.UnhandledException += AppDomain_CurrentDomain_UnhandledException;

            _mutex = new SingleGlobalInstance(); 
            if (_mutex.HasHandle && GetProcesses().Length <= 1)
            {
                Logger.Info(@"Launching UCR");
                _hidGuardianClient = new HidGuardianClient();
                _hidGuardianClient.WhitelistProcess();

                InitializeUcr();
                CheckForBlockedDll();

                _context.ParseCommandLineArguments(e.Args);
                var mw = new MainWindow(_context);
                mw.Show();
            }
            else
            {
                SendArgs(string.Join(";", e.Args));
                Current.Shutdown();
            }
        }

		private void CreateMenuStructure()
		{
			// Show
			var mnuShow = new System.Windows.Forms.MenuItem("Show UCR");
			TrayContextMenu.MenuItems.Add(mnuShow);
			_notify.ContextMenu = TrayContextMenu;
			mnuShow.Visible = true;
			// Hide
			var mnuHide = new System.Windows.Forms.MenuItem("Hide UCR");
			TrayContextMenu.MenuItems.Add(mnuHide);
			_notify.ContextMenu = TrayContextMenu;
			mnuHide.Visible = true;
			// Exit
			var mnuExit = new System.Windows.Forms.MenuItem("Exit");
			TrayContextMenu.MenuItems.Add(mnuExit);
			_notify.ContextMenu = TrayContextMenu;
			mnuExit.Visible = true;
		}

        private void InitializeUcr()
        {
            new ResourceLoader().Load();
            _context = Context.Load();
        }

        private void CheckForBlockedDll()
        {
            if (_context.GetPlugins().Count != 0) return;

            var result = MessageBox.Show(@"UCR has detected blocked files which are required, do you want to unblock blocked UCR files?", "Unblock files?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            var process = new Process
            {
                StartInfo =
                {
                    FileName = "UCR_unblocker.exe",
                    UseShellExecute = true,
                    Arguments = Environment.CurrentDirectory,
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit(1000 * 60 * 5);

            var exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                MessageBox.Show(@"UCR failed to unblock the required files", "Failed to unblock", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }

            InitializeUcr();
        }

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
            _mutex.Dispose();
            _context?.Dispose();
            _hidGuardianClient?.Dispose();
			GC.SuppressFinalize(this);
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Dispose();

			// dispose the context menu
			TrayContextMenu.Dispose();
			
			// dispose the tray icon
			_notify?.Dispose();
		}

        private static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            Logger.Fatal(exception.Message, exception);
        }
    }
}