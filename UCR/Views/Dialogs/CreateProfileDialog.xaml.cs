using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.ViewModels.Dashboard;
using HidWizards.UCR.ViewModels.Dialogs;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class CreateProfileDialog : UserControl
    {
        private CreateProfileDialogViewModel ViewModel { get; set; }

        public CreateProfileDialog(string title, DevicesManager devicesManager)
        {
            ViewModel = new CreateProfileDialogViewModel(title, devicesManager);
            DataContext = ViewModel;
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TextValue.SelectAll();
        }
    }
}
