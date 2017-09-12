using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using Providers;
using UCR.Core;
using UCR.Core.Device;
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

        // DDL
        private ObservableCollection<ComboBoxItemViewModel> Devices { get; set; }
        private ObservableCollection<ComboBoxItemViewModel> DeviceTypes { get; set; }

        // ContextMenu
        private ObservableCollection<ContextMenuItem> BindMenu { get; set; }

        private bool HasLoaded = false;
        

        public DeviceBindingControl()
        {
            BindMenu = new ObservableCollection<ContextMenuItem>();
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceBinding == null) return; // TODO Error logging
            DeviceBindingLabel.Content = Label;
            ReloadGui();
            HasLoaded = true;
        }

        private void ReloadGui()
        {
            LoadDeviceTypeList();
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

        private void LoadDeviceTypeList()
        {
            DeviceTypes = new ObservableCollection<ComboBoxItemViewModel>();
            ComboBoxItemViewModel selectedItem = null;
            foreach (DeviceType deviceType in Enum.GetValues(typeof(DeviceType)))
            {
                var item = new ComboBoxItemViewModel(deviceType.ToString(), deviceType);
                if (deviceType == DeviceBinding.DeviceType) selectedItem = item;
                DeviceTypes.Add(item);
            }
            DeviceTypeBox.ItemsSource = DeviceTypes;
            DeviceTypeBox.SelectedItem = selectedItem;
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
            BuildContextMenu();
            Ddl.ItemsSource = BindMenu;
        }

        private void BuildContextMenu()
        {
            BindMenu = new ObservableCollection<ContextMenuItem>();
            var device = DeviceBinding.Plugin.GetDevice(DeviceBinding);
            if (device == null) return;

            if (device.Bindings == null || device.Bindings.Count == 0)
            {
                var ioController = DeviceBinding.Plugin.ParentProfile.ctx.IOController;
                var list = DeviceBinding.DeviceBindingType == DeviceBindingType.Input 
                    ? ioController.GetInputList() 
                    : ioController.GetOutputList();
                device.Bindings = list[device.ProviderName]?.Devices[device.DeviceHandle]?.Bindings ?? new List<BindingInfo>();
            }
            BindMenu = BuildMenu(device.Bindings);
        }

        private ObservableCollection<ContextMenuItem> BuildMenu(List<BindingInfo> bindingInfos)
        {
            var menuList = new ObservableCollection<ContextMenuItem>();
            if (bindingInfos == null) return menuList;
            foreach (var bindingInfo in bindingInfos)
            {
                RelayCommand cmd = null;
                if (bindingInfo.IsBinding)
                {
                    cmd = new RelayCommand(c =>
                    {
                        DeviceBinding.SetKeyTypeValue((int)bindingInfo.InputType, bindingInfo.InputIndex, bindingInfo.InputSubIndex);
                        LoadBindingName();
                    });
                }
                menuList.Add(new ContextMenuItem(bindingInfo.Title, BuildMenu(bindingInfo.SubBindings), cmd));
            }
            return menuList;
        }

        public DeviceBinding DeviceBinding
        {
            get { return (DeviceBinding)GetValue(DeviceBindingProperty); }
            set { SetValue(DeviceBindingProperty, value); }
        }

        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        private void DeviceNumberBox_OnSelected(object sender, RoutedEventArgs e)
        {
            if (!HasLoaded) return;
            if (DeviceNumberBox.SelectedItem == null) return;
            DeviceBinding.SetDeviceNumber(((ComboBoxItemViewModel)DeviceNumberBox.SelectedItem).Value);
            LoadContextMenu();
            LoadBindingName();
        }

        private void DeviceTypeBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!HasLoaded) return;
            DeviceBinding.SetDeviceType((DeviceType)((ComboBoxItemViewModel) DeviceTypeBox.SelectedItem).Value);
            LoadDeviceInputs();
            LoadContextMenu();
            LoadBindingName();
        }
        
        private void DeviceNumberBox_OnDropDownOpened(object sender, EventArgs e)
        {
            LoadDeviceInputs();
        }
    }
}
