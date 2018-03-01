using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;

namespace HidWizards.UCR.Views
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            VersionTextBlock.Inlines.Add(new Bold(new Run($"Version: {GetVersion()}")));
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
        }

        private string GetVersion()
        {
            return "v0.2.0";
            // TODO
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            return $"v{fvi.FileVersion}";
        }
    }
}
