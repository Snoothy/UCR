using System.ComponentModel.Composition;
using System.Windows;

namespace UCR.Plugins.AxisToButton
{
    [Export(typeof(ResourceDictionary))]
    public partial class Resources : ResourceDictionary
    {
        public Resources()
        {
            InitializeComponent();
        }
    }
}