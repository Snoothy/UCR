using System.ComponentModel.Composition;
using System.Windows;

namespace HidWizards.UCR.Plugins.ButtonToButton
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