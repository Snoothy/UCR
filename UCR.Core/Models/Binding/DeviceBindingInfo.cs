using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HidWizards.UCR.Core.Models.Binding
{
    public class DeviceBindingInfo
    {

        public int KeyType { get; set; }
        public int KeyValue { get; set; }
        public int KeySubValue { get; set; }
        public DeviceBindingCategory DeviceBindingCategory { get; set; }

    }
}
