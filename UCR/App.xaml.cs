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
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private Context context;
        private HidGuardianClient _hidGuardianClient;
        private SingleGlobalInstance mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            AppDomain.CurrentDomain.UnhandledException += AppDomain_CurrentDomain_UnhandledException;

            mutex = new SingleGlobalInstance(); 
            if (mutex.HasHandle && GetProcesses().Length <= 1)
            {
                Logger.Info("Launching UCR");
                _hidGuardianClient = new HidGuardianClient();
                _hidGuardianClient.WhitelistProcess();

                InitializeUcr();
                CheckForBlockedDll();

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

        private void InitializeUcr()
        {
            new ResourceLoader().Load();
            context = Context.Load();
        }

        private void CheckForBlockedDll()
        {
            if (context.GetPlugins().Count != 0) return;

            var result = MessageBox.Show("UCR has detected blocked files which are required, do you want to unblock blocked UCR files?", "Unblock files?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result != MessageBoxResult.Yes) return;

            var process = new Process
            {
                StartInfo =
                {
                    FileName = "UCR_unblocker.exe",
                    UseShellExecute = true,
                    Arguments = $"\"{Environment.CurrentDirectory}\"",
                    CreateNoWindow = true
                }
            };
            process.Start();
            process.WaitForExit(1000 * 60 * 5);

            var exitCode = process.ExitCode;
            if (exitCode != 0)
            {
                MessageBox.Show("UCR failed to unblock the required files", "Failed to unblock", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }

            InitializeUcr();
        }

        private static Process[] GetProcesses()
        {
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        }

        private void SendArgs(string args)
        {
            Logger.Info($"UCR is already running, sending args: {{{args}}}");
            // Find the window with the name of the main form
            var processes = GetProcesses();
            processes = processes.Where(p => p.Id != Process.GetCurrentProcess().Id).ToArray();
            if (processes.Length == 0) return;

            IntPtr ptrCopyData = IntPtr.Zero;
            try
            {
                // Create the data structure and fill with data
                NativeMethods.COPYDATASTRUCT copyData = new NativeMethods.COPYDATASTRUCT
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
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            context?.DevicesManager.UpdateDeviceCache();

            Dispose();
        }

        private static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            Logger.Fatal(exception.Message, exception);
        }
    }
}
