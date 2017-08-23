using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Providers;
using UCR.Models;
using UCR.Models.Devices;
using UCR.Models.Mapping;
using UCR.Models.Plugins;
using UCR.Utilities.Commands;
using UCR.ViewModels;

namespace UCR.Views.Controls
{
    /// <summary>
    /// Interaction logic for DeviceBindingControl.xaml
    /// </summary>
    public partial class DeviceBindingControl : UserControl
    {
        public static readonly DependencyProperty DeviceBindingProperty = DependencyProperty.Register("DeviceBinding", typeof(DeviceBinding), typeof(DeviceBindingControl), new PropertyMetadata(default(DeviceBinding)));
        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register("Label", typeof(string), typeof(DeviceBindingControl), new PropertyMetadata(default(string)));

        // DDL Device number
        private ObservableCollection<ComboBoxItemViewModel> Devices { get; set; }

        // ContextMenu
        private ObservableCollection<ContextMenuItem> BindMenu { get; set; }

        
        
        public DeviceBindingControl()
        {
            LoadDeviceBinding();
            BindMenu = new ObservableCollection<ContextMenuItem>();
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceBinding == null) return; // TODO Error logging
            DeviceBindingLabel.Content = Label;
            ReloadGui();
        }

        private void ReloadGui()
        {
            LoadDeviceList();
            LoadDeviceInputs();
            LoadContextMenu();
            LoadBindingName();
        }

        private void LoadBindingName()
        {
            if (DeviceBinding.IsBound)
            {
                BindButton.Content = DeviceBinding.BoundName();
            }
            else
            {
                BindButton.Content = "Click to enter bind mode";
            }
        }

        private void LoadDeviceList()
        {
            DeviceTypeBox.SelectedItem = DeviceBinding.DeviceType;
        }

        private void LoadDeviceInputs()
        {
            var devicelist = DeviceBinding.Plugin.GetDeviceList(DeviceBinding);
            Devices = new ObservableCollection<ComboBoxItemViewModel>();
            for(var i = 0; i < Math.Max(devicelist?.Count ?? 0, UCRConstants.MaxDevices); i++)
            {
                if (devicelist != null && i < devicelist.Count)
                {
                    Devices.Add(new ComboBoxItemViewModel(i + 1 + ". " + devicelist[i].Title, i));
                }
                else
                {
                    Devices.Add(new ComboBoxItemViewModel(i + 1+". N/A", i));
                }
                
            }

            ComboBoxItemViewModel selectedDevice = null;
            
            foreach (var comboBoxItem in Devices)
            {
                if (comboBoxItem.Value == DeviceBinding.DeviceNumber)
                {
                    selectedDevice = comboBoxItem;
                    break;
                }
            }
            if (selectedDevice == null)
            {
                selectedDevice = new ComboBoxItemViewModel(DeviceBinding.DeviceNumber+1+ ". N/A", DeviceBinding.DeviceNumber);
                Devices.Add(selectedDevice);
            }
            DeviceNumberBox.ItemsSource = Devices;
            DeviceNumberBox.SelectedItem = selectedDevice;
        }

        private void LoadContextMenu()
        {
            if (DeviceBinding == null) return;
            if (DeviceBinding.DeviceBindingType.Equals(DeviceBindingType.Input))
            {
                BuildInputContextMenu();
            }
            else
            {
                BuildOutputContextMenu();
            }
            Ddl.ItemsSource = BindMenu;
        }

        public DeviceBinding DeviceBinding
        {
            get { return (DeviceBinding) GetValue(DeviceBindingProperty); }
            set { SetValue(DeviceBindingProperty, value); }
        }

        public string Label
        {
            get { return (string) GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        private void LoadDeviceBinding()
        {
            // TODO load device binding and update gui accordingly
        }

        private void BuildInputContextMenu()
        {
            BindMenu = new ObservableCollection<ContextMenuItem>();
            var device = DeviceBinding.Plugin.GetDevice(DeviceBinding);
            if (device == null) return;

            switch (DeviceBinding.DeviceType)
            {
                case DeviceType.Keyboard:
                    // TODO
                    break;
                case DeviceType.Mouse:
                    // TODO
                    break;
                case DeviceType.Joystick:
                    BuildSubMenu("Button", device.SupportedButtons.Cast<InputInfo>().ToList(), (int)KeyType.Button);
                    BuildSubMenu("Axis", device.SupportedAxes.FindAll(i => !i.IsUnsigned).Cast<InputInfo>().ToList(), (int)KeyType.Axis);
                    BuildSubMenu("Trigger", device.SupportedAxes.FindAll(i => i.IsUnsigned).Cast<InputInfo>().ToList(), (int)KeyType.Axis);
                    break;
                case DeviceType.Generic:
                    // TODO
                    break;
                default:
                    // TODO Log warning
                    break;
            }
        }

        private void BuildOutputContextMenu()
        {
            BindMenu = new ObservableCollection<ContextMenuItem>();
            var device = DeviceBinding.Plugin.GetDevice(DeviceBinding);
            if (device == null) return;

            switch (DeviceBinding.DeviceType)
            {
                case DeviceType.Keyboard:
                    // TODO
                    break;
                case DeviceType.Mouse:
                    // TODO
                    break;
                case DeviceType.Joystick:
                    BuildSubMenu("Button", device.SupportedButtons.Cast<InputInfo>().ToList(), (int)KeyType.Button);
                    BuildSubMenu("Axis",device.SupportedAxes.FindAll(i => !i.IsUnsigned).Cast<InputInfo>().ToList(), (int)KeyType.Axis);
                    BuildSubMenu("Trigger",device.SupportedAxes.FindAll(i => i.IsUnsigned).Cast<InputInfo>().ToList(), (int)KeyType.Axis);
                    break;
                case DeviceType.Generic:
                    // TODO
                    break;
                default:
                    // TODO Log warning
                    break;
            }
        }

        private void BuildSubMenu(string itemName, List<InputInfo> io, int keyType)
        {
            var topMenu = new ObservableCollection<ContextMenuItem>();
            for (var i = 0; i < io.Count; i++)
            {
                var i1 = i;
                var cmd = new RelayCommand(c =>
                {
                    DeviceBinding.SetKeyTypeValue(keyType, io[i1].Index);
                    LoadBindingName();
                });
                topMenu.Add(new ContextMenuItem(io[i1].Name, new ObservableCollection<ContextMenuItem>(), cmd));
            }
            if (topMenu.Count == 0) return;
            BindMenu.Add(new ContextMenuItem(itemName, topMenu));
        }

        private void DeviceNumberBox_OnSelected(object sender, RoutedEventArgs e)
        {
            if (DeviceNumberBox.SelectedItem == null) return;
            DeviceBinding.SetDeviceNumber(((ComboBoxItemViewModel)DeviceNumberBox.SelectedItem).Value);
            LoadContextMenu();
            LoadBindingName();
        }
    }
}
