using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels;

namespace HidWizards.UCR.Views.Profile
{
    /// <summary>
    /// Interaction logic for ProfileWindow.xaml
    /// </summary>
    public partial class ProfileWindow : Window
    {
        private Context Context { get; set; }
        private Core.Models.Profile Profile { get; set; }
        private bool HasLoaded = false;

        public List<ComboBoxItemViewModel> InputGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputGroups { get; set; }

        public ProfileWindow(Context context, Core.Models.Profile profile)
        {
            Context = context;
            Profile = profile;
            InitializeComponent();
            Title = "Edit " + profile.Title;
            DataContext = Profile;

            PopulateComboBox(InputGroups, DeviceIoType.Input, profile.InputDeviceGroupGuid, InputComboBox);
            PopulateComboBox(OutputGroups, DeviceIoType.Output, profile.OutputDeviceGroupGuid, OutputComboBox);
            Loaded += Window_Loaded;
        }

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

        // TODO Add plugin
        private void AddPlugin_OnClick(object sender, RoutedEventArgs e)
        {
            var win = new PluginDialog(Context, "Add plugin", "Untitled");
            win.ShowDialog();
            if (!win.DialogResult.HasValue || !win.DialogResult.Value) return;
            // TODO Check if plugin with same name exists
            //Profile.AddNewPlugin(win.Plugin, win.TextResult);
            PluginsListBox.Items.Refresh();
            PluginsListBox.SelectedIndex = PluginsListBox.Items.Count - 1;
            PluginsListBox.ScrollIntoView(PluginsListBox.SelectedItem);
        }

        private void RenamePlugin_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin plugin;
            if (!GetSelectedItem(out plugin)) return;
            var win = new TextDialog("Rename plugin", ((Plugin)PluginsListBox.SelectedItem).Title);
            win.ShowDialog();
            if (!win.DialogResult.HasValue || !win.DialogResult.Value) return;
            // TODO Check if plugin with same name exists
            plugin.Rename(win.TextResult);

            PluginsListBox.Items.Refresh();
            PluginsListBox.ScrollIntoView(PluginsListBox.SelectedItem);
        }

        // TODO Remove plugin
        private void RemovePlugin_OnClick(object sender, RoutedEventArgs e)
        {
            Plugin plugin;
            if (!GetSelectedItem(out plugin)) return;
            //Profile.RemovePlugin(plugin);
            PluginsListBox.Items.Refresh();
            PluginsListBox.ScrollIntoView(PluginsListBox.SelectedItem);
        }

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
            HasLoaded = true;
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
            if (!HasLoaded) return;
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
    }
}
