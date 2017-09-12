using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCR.Core;
using UCR.Core.Device;
using UCR.ViewModels;

namespace UCR.Views.Profile
{
    /// <summary>
    /// Interaction logic for ProfileDeviceGroupWindow.xaml
    /// </summary>
    public partial class ProfileDeviceGroupWindow : Window
    {
        private UCRContext ctx;
        private Core.Profile.Profile profile;
        private bool HasLoaded = false;

        public List<ComboBoxItemViewModel> InputJoystickGroups { get; set; }
        public List<ComboBoxItemViewModel> InputKeyboardGroups { get; set; }
        public List<ComboBoxItemViewModel> InputMiceGroups { get; set; }
        public List<ComboBoxItemViewModel> InputGenericGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputJoystickGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputKeyboardGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputMiceGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputGenericGroups { get; set; }

        public ProfileDeviceGroupWindow(UCRContext ctx, Core.Profile.Profile profile)
        {
            this.ctx = ctx;
            this.profile = profile;
            DataContext = this;
            InitializeComponent();

            PopulateComboBox(InputJoystickGroups,  DeviceType.Joystick, DeviceBindingType.Input,  profile.JoystickInputList,  InputJoystickComboBox);
            PopulateComboBox(InputKeyboardGroups,  DeviceType.Keyboard, DeviceBindingType.Input,  profile.KeyboardInputList,  InputKeyboardComboBox);
            PopulateComboBox(InputMiceGroups,      DeviceType.Mouse,    DeviceBindingType.Input,  profile.MiceInputList,      InputMiceComboBox);
            PopulateComboBox(InputGenericGroups,   DeviceType.Generic,  DeviceBindingType.Input,  profile.GenericInputList,   InputGenericComboBox);
            PopulateComboBox(OutputJoystickGroups, DeviceType.Joystick, DeviceBindingType.Output, profile.JoystickOutputList, OutputJoystickComboBox);
            PopulateComboBox(OutputKeyboardGroups, DeviceType.Keyboard, DeviceBindingType.Output, profile.KeyboardOutputList, OutputKeyboardComboBox);
            PopulateComboBox(OutputMiceGroups,     DeviceType.Mouse,    DeviceBindingType.Output, profile.MiceOutputList,     OutputMiceComboBox);
            PopulateComboBox(OutputGenericGroups,  DeviceType.Generic,  DeviceBindingType.Output, profile.GenericOutputList,  OutputGenericComboBox);
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HasLoaded = true;
        }

        private void PopulateComboBox(List<ComboBoxItemViewModel> groups, DeviceType deviceType, DeviceBindingType deviceBindingType, Guid currentGroup, ComboBox comboBox)
        {
            groups = new List<ComboBoxItemViewModel>();
            ComboBoxItemViewModel selectedItem = null;
            groups.Add(new ComboBoxItemViewModel("", new DeviceGroupComboBoxItem()
            {
                DeviceType = deviceType,
                DeviceBindingType = deviceBindingType
            }));
            foreach (var deviceGroup in ctx.GetDeviceGroupList(deviceType))
            {
                var model = new ComboBoxItemViewModel(deviceGroup.Title, new DeviceGroupComboBoxItem()
                {
                    DeviceGroup = deviceGroup,
                    DeviceType = deviceType,
                    DeviceBindingType = deviceBindingType
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
            profile.SetDeviceGroup(value.DeviceBindingType, value.DeviceType, value.DeviceGroup?.Guid ?? Guid.Empty);
        }
    }
}
