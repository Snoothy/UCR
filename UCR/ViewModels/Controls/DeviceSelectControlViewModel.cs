using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Models;
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

        public DeviceSelectControlViewModel(string title, List<DeviceViewModel> devices)
        {
            Title = title;
            Devices = new ObservableCollection<DeviceViewModel>(devices);
            if (Devices.Count != 0) Devices[0].FirstElement = true;
        }

        public List<DeviceViewModel> GetSelectedDevices()
        {
            return Devices.Where(d => d.Checked).ToList();
        }

        public DeviceSelectControlViewModel(string title, List<Device> devices)
        {
            Title = title;
            var result = new ObservableCollection<DeviceViewModel>();

            foreach (var device in devices)
            {
                result.Add(new DeviceViewModel(device));
            }

            if (result.Count > 0) result[0].FirstElement = true;

            Devices = result;
        }
    }
}
