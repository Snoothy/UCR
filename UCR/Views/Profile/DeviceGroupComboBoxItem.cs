using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Core.Device;

namespace UCR.Views.Profile
{
    class DeviceGroupComboBoxItem
    {
        public DeviceGroup DeviceGroup { get; set; }
        public DeviceBindingType DeviceBindingType { get; set; }
        public DeviceType DeviceType { get; set; }
    }
}
