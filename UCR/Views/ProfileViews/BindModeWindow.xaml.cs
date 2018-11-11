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
using System.Windows.Shapes;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Views.ProfileViews
{
    /// <summary>
    /// Interaction logic for BindModeWindow.xaml
    /// </summary>
    public partial class BindModeWindow : Window, IDisposable
    {
        public BindModeWindow(DeviceBinding deviceBinding)
        {
            InitializeComponent();
        }

        public void Dispose()
        {

        }
    }
}
