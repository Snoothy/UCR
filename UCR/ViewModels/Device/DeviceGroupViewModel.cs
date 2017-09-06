using System;
using System.Collections;
using System.Collections.Generic;
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
        public ICollection<DeviceGroupViewModel> Groups { get; set; }
        public ICollection<Models.Devices.Device> Devices { get; set; }

        public DeviceGroupViewModel(string title = null, Guid guid = new Guid())
        {
            Title = title;
            Groups = new List<DeviceGroupViewModel>();
            Devices = new List<Models.Devices.Device>();
            if (Guid.Equals(Guid.Empty)) Guid = Guid.NewGuid();
        }

        public DeviceGroupViewModel(DeviceGroup deviceGroup) : this()
        {
            Title = deviceGroup.Title;
            Devices = deviceGroup.Devices;
            Guid = deviceGroup.Guid;
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
    }
    
}
