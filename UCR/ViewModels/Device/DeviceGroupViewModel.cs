using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UCR.Models.Devices;

namespace UCR.ViewModels.Device
{
    public class DeviceGroupViewModel
    {
        public string Title { get; set; }
        public Guid Guid { get; set; }
        public ObservableCollection<DeviceGroupViewModel> Groups { get; set; }
        public ObservableCollection<Models.Devices.Device> Devices { get; set; }

        public DeviceGroupViewModel(string title = null, Guid guid = new Guid())
        {
            Title = title;
            Groups = new ObservableCollection<DeviceGroupViewModel>();
            Devices = new ObservableCollection<Models.Devices.Device>();
            Guid = guid.Equals(Guid.Empty) ? Guid.NewGuid() : guid;
        }

        public DeviceGroupViewModel(DeviceGroup deviceGroup) : this(deviceGroup.Title, deviceGroup.Guid)
        {
            deviceGroup.Devices.ForEach(d => Devices.Add(d));
        }

        public IList Items
        {
            get
            {
                var items = new CompositeCollection
                {
                    new CollectionContainer {Collection = Groups},
                    new CollectionContainer {Collection = Devices}
                };
                return items;
            }
        }

        public static DeviceGroupViewModel FindDeviceGroupViewModelWithDevice(Collection<DeviceGroupViewModel> deviceGroupViewModels, Models.Devices.Device device)
        {
            DeviceGroupViewModel result = null;
            foreach (var deviceGroupViewModel in deviceGroupViewModels)
            {
                if (deviceGroupViewModel.Devices.Contains(device)) result = deviceGroupViewModel;
            }
            return result;
        }
    }
    
}
