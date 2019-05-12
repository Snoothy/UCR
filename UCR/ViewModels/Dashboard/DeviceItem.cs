using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class DeviceItem
    {
        public string Title => Device.GetFullTitleForProfile(Profile);
        public string ProviderName => Device.ProviderName;

        public Device Device { get; set; }
        private Profile Profile { get; set; }

        public DeviceItem(Device device, Profile profile)
        {
            Device = device;
            Profile = profile;
        }
    }
}