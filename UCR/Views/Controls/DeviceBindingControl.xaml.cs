using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.Core.Models.Binding;
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
        public static readonly DependencyProperty CategoryProperty = DependencyProperty.Register("Category", typeof(DeviceBindingCategory?), typeof(DeviceBindingControl), new PropertyMetadata(default(DeviceBindingCategory?)));
        
        /* ContextMenu */
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
            ReloadGui();
            HasLoaded = true;
        }

        private void ReloadGui()
        {
            LoadContextMenu();
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
            var deviceConfiguration = GetSelectedDeviceConfiguration();
            if (deviceConfiguration == null) return;
            BindMenu = BuildMenu(deviceConfiguration.Device.GetDeviceBindingMenu(DeviceBinding.Profile.Context, DeviceBinding.DeviceIoType));
            BindMenu.Add(CreateClearCommandMenuItem());
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
                    if (Category != null && deviceBindingNode.DeviceBindingInfo.DeviceBindingCategory != Category) continue;
                    cmd = new RelayCommand(c =>
                    {
                        DeviceBinding.SetDeviceConfigurationGuid(GetSelectedDeviceConfiguration().Guid);
                        DeviceBinding.SetKeyTypeValue(deviceBindingNode.DeviceBindingInfo.KeyType, deviceBindingNode.DeviceBindingInfo.KeyValue, deviceBindingNode.DeviceBindingInfo.KeySubValue);
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

        private ContextMenuItem CreateClearCommandMenuItem()
        {
            var clearCommand = new RelayCommand(c => { DeviceBinding.ClearBinding(); });
            return new ContextMenuItem("Clear binding", null, clearCommand);
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
            if (DeviceSelectionBox.SelectedItem == null) return;
            DeviceBinding.SetDeviceConfigurationGuid(GetSelectedDeviceConfiguration().Guid);
            LoadContextMenu();
        }

        private DeviceConfiguration GetSelectedDeviceConfiguration()
        {
            Guid guid = ((ComboBoxItemViewModel) DeviceSelectionBox.SelectedItem).Value;
            return DeviceBinding.Profile.GetDeviceConfiguration(DeviceBinding.DeviceIoType, guid);
        }

        private void BindButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (e is KeyboardEventArgs && !((KeyEventArgs)e).Key.Equals(Key.Space))
            {
                e.Handled = true;
                return;
            }
            if (DeviceBinding.DeviceIoType.Equals(DeviceIoType.Input))
            {
                if (DeviceBinding.IsInBindMode) return;
                if (Category.HasValue) DeviceBinding.DeviceBindingCategory = Category.Value;
                DeviceBinding.EnterBindMode();
            }
            else
            {
                OpenContextMenu();
            }
        }

        private void BindMenuButton_OnClick(object sender, RoutedEventArgs e)
        {
            OpenContextMenu();
        }

        private void OpenContextMenu()
        {
            if (DeviceBinding.IsInBindMode) return;
            var contextMenu = BindButton.ContextMenu;
            contextMenu.PlacementTarget = BindButton;
            contextMenu.IsOpen = true;
        }
    }
}
