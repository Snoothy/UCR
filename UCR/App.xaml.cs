using System.Collections.Generic;
using System.Windows;
using UCR.Core;
using UCR.Utilities;
using UCR.Views;

namespace UCR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Context context;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            new ResourceLoader().Load();
            context = Context.Load();
            context.ParseCommandLineArguments(e.Args);
            var mw = new MainWindow(context);
            mw.Show();
        }
    }
}
