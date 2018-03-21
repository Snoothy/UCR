using System;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class DeviceSubscription
    {
        public Guid DeviceSubscriptionGuid { get; }
        public Device Device { get; }
        public Profile Profile { get; }

        public DeviceSubscription(Device device, Profile profile)
        {
            DeviceSubscriptionGuid = Guid.NewGuid();
            Device = device;
            Profile = profile;
        }
    }
}
