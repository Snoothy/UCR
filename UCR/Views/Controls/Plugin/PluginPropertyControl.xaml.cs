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

namespace HidWizards.UCR.Views.Controls.Plugin
{
    public partial class PluginPropertyControl : UserControl
    {
        public static readonly DependencyProperty PropertyContentProperty = DependencyProperty.Register("PropertyContent", typeof(object), typeof(PluginPropertyControl), new PropertyMetadata(default(object)));

        public PluginPropertyControl()
        {
            InitializeComponent();
        }

        public object PropertyContent
        {
            get { return GetValue(PropertyContentProperty); }
            set { SetValue(PropertyContentProperty, value); }
        }
    }
}
