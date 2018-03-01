using System;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class DeviceSubscription
    {
        public Guid DeviceSubscriptionGuid { get; }
        public Device.Device Device { get; }
        public Profile.Profile Profile { get; }

        public DeviceSubscription(Device.Device device, Profile.Profile profile)
        {
            DeviceSubscriptionGuid = Guid.NewGuid();
            Device = device;
            Profile = profile;
        }
    }
}
