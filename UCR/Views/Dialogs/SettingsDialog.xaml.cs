using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.ViewModels.Settings;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class SettingsDialog : UserControl
    {
        public SettingsViewModel ViewModel { get; }

        public SettingsDialog(SettingsManager settingsManager)
        {
            ViewModel = new SettingsViewModel(settingsManager);
            DataContext = ViewModel;
            
            InitializeComponent();
        }
    }
}
