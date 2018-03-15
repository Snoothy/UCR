using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.Core.Utilities;
using HidWizards.UCR.Utilities.Commands;
using HidWizards.UCR.ViewModels;

namespace HidWizards.UCR.Views.Controls
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

        // ContextMenu
        private ObservableCollection<ContextMenuItem> BindMenu { get; set; }

        private bool HasLoaded = false;
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(DeviceBindingCategory?), typeof(DeviceBindingControl), new PropertyMetadata(default(DeviceBindingCategory?)));


        public DeviceBindingControl()
        {
            BindMenu = new ObservableCollection<ContextMenuItem>();
            InitializeComponent();
            Loaded += UserControl_Loaded;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DeviceBinding == null) return; // TODO Error logging
            DeviceBindingLabel.Header = Label;
            ReloadGui();
            HasLoaded = true;
        }

        private void ReloadGui()
        {
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
                BindButton.Content = "Click to Bind";
            }
        }

        private void LoadDeviceInputs()
        {
            var devicelist = DeviceBinding.Profile.GetDeviceList(DeviceBinding);
            Devices = new ObservableCollection<ComboBoxItemViewModel>();
            for(var i = 0; i < Math.Max(devicelist?.Count ?? 0, Constants.MaxDevices); i++)
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
            var device = DeviceBinding.Profile.GetDevice(DeviceBinding);
            if (device == null) return;
            BindMenu = BuildMenu(device.GetDeviceBindingMenu(DeviceBinding.Profile.Context, DeviceBinding.DeviceIoType));
        }

        private ObservableCollection<ContextMenuItem> BuildMenu(List<DeviceBindingNode> deviceBindingNodes)
        {
            var menuList = new ObservableCollection<ContextMenuItem>();
            if (deviceBindingNodes == null) return menuList;
            foreach (var deviceBindingNode in deviceBindingNodes)
            {
                
                RelayCommand cmd = null;
                if (deviceBindingNode.IsBinding)
                {
                    if (Category != null && deviceBindingNode.DeviceBinding.DeviceBindingCategory != Category) continue;
                    cmd = new RelayCommand(c =>
                    {
                        DeviceBinding.SetKeyTypeValue(deviceBindingNode.DeviceBinding.KeyType, deviceBindingNode.DeviceBinding.KeyValue, deviceBindingNode.DeviceBinding.KeySubValue);
                        LoadBindingName();
                    });
                }
                var menu = new ContextMenuItem(deviceBindingNode.Title, BuildMenu(deviceBindingNode.ChildrenNodes), cmd);
                if (deviceBindingNode.IsBinding || !deviceBindingNode.IsBinding && menu.Children.Count > 0)
                {
                    menuList.Add(menu);
                }
                
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

        public DeviceBindingCategory? Category
        {
            get { return (DeviceBindingCategory?) GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        private void DeviceNumberBox_OnSelected(object sender, RoutedEventArgs e)
        {
            if (!HasLoaded) return;
            if (DeviceNumberBox.SelectedItem == null) return;
            DeviceBinding.SetDeviceNumber(((ComboBoxItemViewModel)DeviceNumberBox.SelectedItem).Value);
            LoadContextMenu();
            LoadBindingName();
        }
        
        private void DeviceNumberBox_OnDropDownOpened(object sender, EventArgs e)
        {
            LoadDeviceInputs();
        }

        private void BindButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var contextMenu = button.ContextMenu;
            contextMenu.PlacementTarget = button;
            contextMenu.IsOpen = true;
        }
    }
}
