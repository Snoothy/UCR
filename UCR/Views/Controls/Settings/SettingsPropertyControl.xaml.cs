using System.Windows;
using System.Windows.Controls;

namespace HidWizards.UCR.Views.Controls.Settings
{
    public partial class SettingsPropertyControl : UserControl
    {
        public static readonly DependencyProperty PropertyContentProperty = DependencyProperty.Register("PropertyContent", typeof(object), typeof(SettingsPropertyControl), new PropertyMetadata(default(object)));

        public SettingsPropertyControl()
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
