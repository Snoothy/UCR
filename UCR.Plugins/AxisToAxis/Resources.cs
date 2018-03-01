using System.ComponentModel.Composition;
using System.Windows;

namespace HidWizards.UCR.Plugins.AxisToAxis
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