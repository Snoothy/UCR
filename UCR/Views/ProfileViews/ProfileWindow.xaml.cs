using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels;
using HidWizards.UCR.ViewModels.ProfileViewModels;

namespace HidWizards.UCR.Views.ProfileViews
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private Context Context { get; }
        private Profile Profile { get; }
        private ProfileViewModel ProfileViewModel { get; }
        private bool _hasLoaded;

        public List<ComboBoxItemViewModel> InputGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputGroups { get; set; }

        public ProfileWindow(Context context, Profile profile)
        {
            Context = context;
            Profile = profile;
            ProfileViewModel = new ProfileViewModel(profile);
            InitializeComponent();
            Title = "Edit " + profile.Title;
            DataContext = ProfileViewModel;

            PopulateComboBox(InputGroups, DeviceIoType.Input, profile.InputDeviceGroupGuid, InputComboBox);
            PopulateComboBox(OutputGroups, DeviceIoType.Output, profile.OutputDeviceGroupGuid, OutputComboBox);
            Loaded += Window_Loaded;
        }

        #region Profile

        private void ActivateProfile(object sender, RoutedEventArgs e)
        {
            if (!Profile.ActivateProfile())
            {
                MessageBox.Show("The profile could not be activated, see the log for more details", "Profile failed to activate!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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
            plugin.SetProfile(Profile);

            ProfileViewModel.SelectedMapping.AddPlugin(plugin);

            if (ProfileViewModel.SelectedMapping.Plugins.Count == 1)
            {
                PopulatePluginsComboBox();
            }

            PluginsListBox.Items.Refresh();
            PluginsListBox.SelectedIndex = PluginsListBox.Items.Count - 1;
            PluginsListBox.ScrollIntoView(PluginsListBox.SelectedItem);
        }
        
        #endregion

        private bool GetSelectedItem(out Plugin selection)
        {
            var item = PluginsListBox.SelectedItem as Plugin;
            if (item == null)
            {
                MessageBox.Show("Please select a plugin", "No plugin selected!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                selection = null;
                return false;
            }
            selection = item;
            return true;
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _hasLoaded = true;
            if (ProfileViewModel.MappingsList.Count > 0)
            {
                MappingsListBox.SelectedItem = ProfileViewModel.MappingsList[0];
            }
        }

        private void PopulateComboBox(List<ComboBoxItemViewModel> groups, DeviceIoType deviceIoType, Guid currentGroup, ComboBox comboBox)
        {
            groups = new List<ComboBoxItemViewModel>();
            ComboBoxItemViewModel selectedItem = null;

            groups.Add(new ComboBoxItemViewModel(GetInheritedDeviceGroupName(deviceIoType), new DeviceGroupComboBoxItem()
            {
                DeviceIoType = deviceIoType
            }));
            foreach (var deviceGroup in Context.DeviceGroupsManager.GetDeviceGroupList(deviceIoType))
            {
                var model = new ComboBoxItemViewModel(deviceGroup.Title, new DeviceGroupComboBoxItem()
                {
                    DeviceGroup = deviceGroup,
                    DeviceIoType = deviceIoType
                });
                groups.Add(model);
                if (deviceGroup.Guid == currentGroup) selectedItem = model;
            }
            comboBox.ItemsSource = groups;
            if (selectedItem != null)
            {
                comboBox.SelectedItem = selectedItem;
            }
            else
            {
                comboBox.SelectedItem = groups[0];
            }
        }

        private void DeviceGroup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_hasLoaded) return;
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox?.SelectedItem as ComboBoxItemViewModel;
            if (selectedItem == null) return;
            var value = selectedItem.Value as DeviceGroupComboBoxItem;
            Profile.SetDeviceGroup(value.DeviceIoType, value.DeviceGroup?.Guid ?? Guid.Empty);
            PluginsListBox.Items.Refresh();
        }

        private string GetInheritedDeviceGroupName(DeviceIoType deviceIoType)
        {
            var parentDeviceGroupName = "None";
            var parentDeviceGroup = Profile.ParentProfile?.GetDeviceGroup(deviceIoType);
            if (parentDeviceGroup != null) parentDeviceGroupName = parentDeviceGroup.Title;
            if (Profile.ParentProfile != null) parentDeviceGroupName += " (Inherited)";
            return parentDeviceGroupName;
        }

        private void MappingsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            var mappingViewModel = listBox.SelectedItem as MappingViewModel;
            ProfileViewModel.SelectedMapping = mappingViewModel;
            if (mappingViewModel == null) return;

            PopulatePluginsComboBox();
            PluginsListBox.ItemsSource = ProfileViewModel.SelectedMapping.Plugins;
            PluginsListBox.Items.Refresh();
        }

        private void PluginsListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PluginsListBox.Items.Count == 0)
            {
                PopulatePluginsComboBox();
            }
        }
    }
}
