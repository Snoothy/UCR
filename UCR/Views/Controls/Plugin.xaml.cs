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

namespace UCR.Views.Controls
{
    /// <summary>
    /// Interaction logic for Plugin.xaml
    /// </summary>
    public partial class Plugin : UserControl
    {
        public static readonly DependencyProperty PluginViewProperty = DependencyProperty.Register("PluginView", typeof(object), typeof(Plugin), new PropertyMetadata(default(object)));
        public static readonly DependencyProperty PluginTitleProperty = DependencyProperty.Register("PluginTitle", typeof(object), typeof(Plugin), new PropertyMetadata(default(object)));

        public Plugin()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }

        public object PluginView
        {
            get { return (object) GetValue(PluginViewProperty); }
            set { SetValue(PluginViewProperty, value); }
        }

        public object PluginTitle
        {
            get { return (object) GetValue(PluginTitleProperty); }
            set { SetValue(PluginTitleProperty, value); }
        }
    }
}
