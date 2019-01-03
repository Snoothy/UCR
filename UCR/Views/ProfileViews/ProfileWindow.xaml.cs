using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels;
using HidWizards.UCR.ViewModels.ProfileViewModels;
using UCR.Views.ProfileViews;

namespace HidWizards.UCR.Views.ProfileViews
{
    public partial class ProfileWindow : Window
    {
        public Guid ProfileGuid => Profile.Guid;
        private Context Context { get; }
        private Profile Profile { get; }
        private ProfileViewModel ProfileViewModel { get; }

        public ProfileWindow(Context context, Profile profile)
        {
            Context = context;
            Profile = profile;
            ProfileViewModel = new ProfileViewModel(profile);
            InitializeComponent();
            Title = "Edit " + profile.Title;
            DataContext = ProfileViewModel;

            Loaded += Window_Loaded;
        }

        private void Save_OnExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            Context.SaveContext();
        }

        private void Save_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Context.IsNotSaved;
        }

        #region Profile

        private void ActivateProfile(object sender, RoutedEventArgs e)
        {
            if (!Profile.ActivateProfile())
            {
                MessageBox.Show("The Profile could not be activated, see the log for more details", "Profile failed to activate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void DeactivateProfile(object sender, RoutedEventArgs e)
        {
            Profile.Deactivate();
        }

        #endregion

        #region Mappings

        private void AddMapping_OnClick(object sender, RoutedEventArgs e)
        {
            AddMappingFromText();
        }

        private void AddMappingFromText()
        {
            var title = MappingNameField.Text;
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please write a title to add a new mapping", "No title!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            ProfileViewModel.AddMapping(title);
            MappingNameField.Text = "";
            MappingsListBox.SelectedIndex = MappingsListBox.Items.Count - 1;
            MappingsListBox.ScrollIntoView(MappingsListBox.SelectedItem);
            MappingNameField.Focus();
        }

        #endregion

        #region Plugin

        private void PopulatePluginsComboBox()
        {
            var pluginlist = ProfileViewModel.SelectedMapping.Mapping.GetPluginList();
            var plugins = new ObservableCollection<ComboBoxItemViewModel>();
            foreach (var plugin in pluginlist)
            {
                plugins.Add(new ComboBoxItemViewModel(plugin.PluginName, plugin));
            }
            PluginsComboBox.ItemsSource = plugins;
            PluginsComboBox.SelectedIndex = 0;
        }

        private void AddPlugin_OnClick(object sender, RoutedEventArgs e)
        {
            var plugin = ((ComboBoxItemViewModel)PluginsComboBox.SelectedItem)?.Value;
            if (plugin == null) return;

            ProfileViewModel.SelectedMapping.AddPlugin(Profile.Context.PluginManager.GetNewPlugin(plugin), null);

            if (ProfileViewModel.SelectedMapping.Plugins.Count == 1)
            {
                PopulatePluginsComboBox();
            }
        }

        #endregion
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (ProfileViewModel.MappingsList.Count > 0)
            {
                MappingsListBox.SelectedItem = ProfileViewModel.MappingsList[0];
            }
        }

        private void ManageDeviceGroups_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new ProfileDeviceGroupWindow(Context, Profile);
            Action showAction = () => win.Show();
            Dispatcher.BeginInvoke(showAction);
        }

        private void MappingNameField_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddMappingFromText();
            }
        }
    }
}
