using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace UCR.ViewModels.Device
{
    public class DeviceGroupViewModel
    {
        public string Name { get; set; }
        public ICollection<DeviceGroupViewModel> Groups { get; set; }
        public ICollection<Models.Devices.Device> Devices { get; set; }

        public DeviceGroupViewModel()
        {
            Groups = new List<DeviceGroupViewModel>();
            Devices = new List<Models.Devices.Device>();
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
