using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.Dashboard
{
    public class DeviceItem
    {
        public string Title => DeviceConfiguration.ConfigurationName ?? DeviceConfiguration.Device.GetFullTitleForProfile(Profile);
        public string ProviderName => DeviceConfiguration.Device.ProviderName;

        public DeviceConfiguration DeviceConfiguration { get; set; }
        private Profile Profile { get; set; }

        public DeviceItem(DeviceConfiguration deviceConfiguration, Profile profile)
        {
            DeviceConfiguration = deviceConfiguration;
            Profile = profile;
        }
    }
}