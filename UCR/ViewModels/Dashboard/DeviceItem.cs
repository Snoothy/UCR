using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class DeviceItem
    {
        public string Title => Device.Title;
        public string ProviderName => Device.ProviderName;

        private Device Device { get; set; }

        public DeviceItem(Device device)
        {
            Device = device;
        }
    }
}