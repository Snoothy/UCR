using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using UCR.Core;
using UCR.Core.Models.Binding;
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

        public List<ComboBoxItemViewModel> InputJoystickGroups { get; set; }
        public List<ComboBoxItemViewModel> InputKeyboardGroups { get; set; }
        public List<ComboBoxItemViewModel> InputMiceGroups { get; set; }
        public List<ComboBoxItemViewModel> InputGenericGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputJoystickGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputKeyboardGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputMiceGroups { get; set; }
        public List<ComboBoxItemViewModel> OutputGenericGroups { get; set; }

        public ProfileDeviceGroupWindow(Context context, Core.Models.Profile.Profile profile)
        {
            this.context = context;
            this.profile = profile;
            DataContext = this;
            InitializeComponent();

            PopulateComboBox(InputJoystickGroups,  DeviceType.Joystick, DeviceIoType.Input,  profile.JoystickInputList,  InputJoystickComboBox);
            PopulateComboBox(InputKeyboardGroups,  DeviceType.Keyboard, DeviceIoType.Input,  profile.KeyboardInputList,  InputKeyboardComboBox);
            PopulateComboBox(InputMiceGroups,      DeviceType.Mouse,    DeviceIoType.Input,  profile.MiceInputList,      InputMiceComboBox);
            PopulateComboBox(InputGenericGroups,   DeviceType.Generic,  DeviceIoType.Input,  profile.GenericInputList,   InputGenericComboBox);
            PopulateComboBox(OutputJoystickGroups, DeviceType.Joystick, DeviceIoType.Output, profile.JoystickOutputList, OutputJoystickComboBox);
            PopulateComboBox(OutputKeyboardGroups, DeviceType.Keyboard, DeviceIoType.Output, profile.KeyboardOutputList, OutputKeyboardComboBox);
            PopulateComboBox(OutputMiceGroups,     DeviceType.Mouse,    DeviceIoType.Output, profile.MiceOutputList,     OutputMiceComboBox);
            PopulateComboBox(OutputGenericGroups,  DeviceType.Generic,  DeviceIoType.Output, profile.GenericOutputList,  OutputGenericComboBox);
            Loaded += Window_Loaded;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HasLoaded = true;
        }

        private void PopulateComboBox(List<ComboBoxItemViewModel> groups, DeviceType deviceType, DeviceIoType deviceIoType, Guid currentGroup, ComboBox comboBox)
        {
            groups = new List<ComboBoxItemViewModel>();
            ComboBoxItemViewModel selectedItem = null;
            groups.Add(new ComboBoxItemViewModel("", new DeviceGroupComboBoxItem()
            {
                DeviceType = deviceType,
                DeviceIoType = deviceIoType
            }));
            foreach (var deviceGroup in context.DeviceGroupsManager.GetDeviceGroupList(deviceType))
            {
                var model = new ComboBoxItemViewModel(deviceGroup.Title, new DeviceGroupComboBoxItem()
                {
                    DeviceGroup = deviceGroup,
                    DeviceType = deviceType,
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
            profile.SetDeviceGroup(value.DeviceIoType, value.DeviceType, value.DeviceGroup?.Guid ?? Guid.Empty);
        }
    }
}
