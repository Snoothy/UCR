using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace HidWizards.UCR.Views.Dialogs
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutDialog : UserControl
    {
        public AboutDialog()
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
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return fileVersionInfo.ProductVersion;
        }
    }
}
