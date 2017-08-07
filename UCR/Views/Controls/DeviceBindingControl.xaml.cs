using System;
using System.Collections.Generic;
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
using UCR.Models.Mapping;

namespace UCR.Views.Controls
{
    /// <summary>
    /// Interaction logic for DeviceBindingControl.xaml
    /// </summary>
    public partial class DeviceBindingControl : UserControl
    {
        public static readonly DependencyProperty DeviceBindingProperty = DependencyProperty.Register("DeviceBinding", typeof(DeviceBinding), typeof(DeviceBindingControl), new PropertyMetadata(default(DeviceBinding)));
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DeviceBindingControl), new PropertyMetadata(default(string)));

        public DeviceBindingControl()
        {
            InitializeComponent();
        }

        public DeviceBinding DeviceBinding
        {
            get { return (DeviceBinding) GetValue(DeviceBindingProperty); }
            set { SetValue(DeviceBindingProperty, value); }
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
    }
}
