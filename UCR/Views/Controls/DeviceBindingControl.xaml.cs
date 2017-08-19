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
        private ObservableCollection<ComboBoxItem> Devices { get; set; }
        private ComboBoxItem SelectedDevice { get; set; }

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
            LoadDevices();
            LoadContextMenu();
        }

        private void LoadDevices()
        {
            DeviceBinding.Plugin.GetDeviceList(DeviceBinding.DeviceType);
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

            switch (DeviceBinding.DeviceType)
            {
                case DeviceType.Keyboard:
                    // TODO
                    break;
                case DeviceType.Mouse:
                    // TODO
                    break;
                case DeviceType.Joystick:
                    var device = DeviceBinding.Plugin.GetDevice(DeviceBinding) as Joystick;
                    if (device.MaxButtons > 0) BindMenu.Add(new ContextMenuItem("Buttons", BuildButtonSubMenu(device.MaxButtons, (int)KeyType.Button)));
                    break;
                case DeviceType.Generic:
                    // TODO
                    break;
                case null:
                default:
                    // TODO Log warning
                    break;
            }
        }

        private ObservableCollection<ContextMenuItem> BuildButtonSubMenu(int numberOfButtons, int keyType)
        {
            var topMenu = new ObservableCollection<ContextMenuItem>();
            for (var i = 0; i < numberOfButtons; i++)
            {
                var i1 = i;
                var cmd = new RelayCommand(c => DeviceBinding.SetKeyTypeValue(keyType, i1));
                topMenu.Add(new ContextMenuItem((i+1).ToString(), new ObservableCollection<ContextMenuItem>(), cmd));
            }
            return topMenu;
        }

        private void BuildOutputContextMenu()
        {
            BindMenu = new ObservableCollection<ContextMenuItem>();
            // TODO
        }


    }
}
