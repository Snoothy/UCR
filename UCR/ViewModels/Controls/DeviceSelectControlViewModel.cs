using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.ViewModels.DeviceViewModels;

namespace HidWizards.UCR.ViewModels.Controls
{
    public class DeviceSelectControlViewModel
    {
        public string Title { get; set; }
        public ObservableCollection<DeviceViewModel> Devices { get; set; }

        public DeviceSelectControlViewModel()
        {
        }

        public DeviceSelectControlViewModel(string title, ObservableCollection<DeviceViewModel> devices)
        {
            Title = title;
            Devices = devices;
        }
    }
}
