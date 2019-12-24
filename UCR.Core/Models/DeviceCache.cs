using System.Collections.Generic;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models
{
    public class DeviceCache
    {

        public string Title { get; set; }
        public string ProviderName { get; set; }
        public string DeviceHandle { get; set; }
        public int DeviceNumber { get; set; }
        public List<DeviceBindingNode> DeviceBindingMenu { get; set; }
        public bool Blockable { get; set; }

    }
}
