using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models.Settings;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Utilities;
using HidWizards.UCR.Views;
using NamedPipeWrapper;
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
        private bool StartMinimized;

        private NamedPipeServer<NamedPipeMessage> namedPipeServer;
        private string NamedPipeName => "UCR_NamedPipe";

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
                StartNamedPipeServer();

                var mw = new MainWindow(context);
                context.MinimizedToTrayEvent += Context_MinimizedToTrayEvent;
                context.ParseCommandLineArguments(e.Args);
                if (!StartMinimized && SettingsCollection.LaunchMinimized) context.MinimizeToTray();
                if (!StartMinimized) mw.Show();
            }
            else
            {
                SendIpcArgs(e.Args);
                Current.Shutdown();
            }
        }

        private void StartNamedPipeServer()
        {
            namedPipeServer = new NamedPipeServer<NamedPipeMessage>(NamedPipeName);
            namedPipeServer.ClientMessage += ServerOnClientMessage;
            namedPipeServer.Start();
        }

        private void SendIpcArgs(IEnumerable<string> args)
        {
            var client = new NamedPipeClient<NamedPipeMessage>(NamedPipeName);
            client.Start();
            client.WaitForConnection();
            client.PushMessage(new NamedPipeMessage()
            {
                Commands = new List<string>(args)
            });
            client.Stop();
        }

        private void ServerOnClientMessage(NamedPipeConnection<NamedPipeMessage, NamedPipeMessage> connection, NamedPipeMessage message)
        {
            context.ParseCommandLineArguments(message.Commands);
        }

        private void Context_MinimizedToTrayEvent()
        {
            StartMinimized = true;
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

        public void Dispose()
        {
            mutex.Dispose();
            context?.Dispose();
            _hidGuardianClient?.Dispose();
        }

        private void App_OnExit(object sender, ExitEventArgs e)
        {
            context?.DevicesManager.UpdateDeviceCache();
            namedPipeServer?.Stop();
            Dispose();
        }

        private static void AppDomain_CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = (Exception) e.ExceptionObject;
            Logger.Fatal(exception.Message, exception);
        }
    }
}
