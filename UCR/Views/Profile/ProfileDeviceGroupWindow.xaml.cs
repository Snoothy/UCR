using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCR.Core;
using UCR.Core.Models.Device;
using UCR.ViewModels;

namespace UCR.Views.Profile
{
    /// <summary>
    /// Interaction logic for ProfileDeviceGroupWindow.xaml
    /// </summary>
    public partial class ProfileDeviceGroupWindow : Window
    {
        private Context context;
        private Core.Models.Profile.Profile profile;
        private bool HasLoaded = false;

        public List<ComboBoxItemViewModel> InputGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputGroups { get; set; }

        public ProfileDeviceGroupWindow(Context context, Core.Models.Profile.Profile profile)
        {
            this.context = context;
            this.profile = profile;
            DataContext = this;
            InitializeComponent();

            PopulateComboBox(InputGroups, DeviceIoType.Input,  profile.InputDeviceGroupGuid,  InputComboBox);
            PopulateComboBox(OutputGroups, DeviceIoType.Output, profile.OutputDeviceGroupGuid, OutputComboBox);
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HasLoaded = true;
        }

        private void PopulateComboBox(List<ComboBoxItemViewModel> groups, DeviceIoType deviceIoType, Guid currentGroup, ComboBox comboBox)
        {
            groups = new List<ComboBoxItemViewModel>();
            ComboBoxItemViewModel selectedItem = null;
            groups.Add(new ComboBoxItemViewModel("", new DeviceGroupComboBoxItem()
            {
                DeviceIoType = deviceIoType
            }));
            foreach (var deviceGroup in context.DeviceGroupsManager.GetDeviceGroupList(deviceIoType))
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
            if (selectedItem != null) comboBox.SelectedItem = selectedItem;
        }

        private void DeviceGroup_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!HasLoaded) return;
            var comboBox = sender as ComboBox;
            var selectedItem = comboBox?.SelectedItem as ComboBoxItemViewModel;
            if (selectedItem == null) return;
            var value = selectedItem.Value as DeviceGroupComboBoxItem;
            profile.SetDeviceGroup(value.DeviceIoType, value.DeviceGroup?.Guid ?? Guid.Empty);
        }
    }
}
