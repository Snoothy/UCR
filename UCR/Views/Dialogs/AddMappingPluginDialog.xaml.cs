using System;
using System.Collections.Generic;
using System.Windows.Controls;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.Dashboard;
using HidWizards.UCR.ViewModels.ProfileViewModels;

namespace HidWizards.UCR.Views.Dialogs
{
    public partial class AddMappingPluginDialog : UserControl
    {
        private AddMappingPluginDialogViewModel ViewModel { get; }

        public AddMappingPluginDialog(MappingViewModel mappingViewModel)
        {
            ViewModel = new AddMappingPluginDialogViewModel(mappingViewModel);
            DataContext = ViewModel;

            InitializeComponent();
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ViewModel.SelectedPlugin = (SimplePluginViewModel)PluginList.SelectedItem;
            ViewModel.SelectionChanged();
        }
    }
}
