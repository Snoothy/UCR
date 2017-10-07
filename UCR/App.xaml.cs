using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using UCR.Core;
using UCR.Utilities;
using UCR.Views;
using Application = System.Windows.Application;

namespace UCR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        private Context context;
        private HidGuardianClient _hidGuardianClient;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            var mutex = new SingleGlobalInstance(); 
            if (mutex.HasHandle && GetProcesses().Length <= 1)
            {
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

        private static Process[] GetProcesses()
        {
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName);
        }

        private void SendArgs(string args)
        {
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
            catch (Exception)
            {
                // TODO log exception
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
            context?.Dispose();
            _hidGuardianClient?.Dispose();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            Dispose();
        }
    }
}
