using System;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class InputSubscription
    {
        public DeviceBinding DeviceBinding { get; }
        public Profile Profile { get; }
        public Guid SubscriptionStateGuid { get; set; }
        public Guid DeviceBindingSubscriptionGuid { get; set; }
        public bool IsOverwritten { get; set; }
        public DeviceSubscription DeviceSubscription { get; }

        public InputSubscription(DeviceBinding deviceBinding, Profile profile, Guid subscriptionStateGuid)
        {
            DeviceBinding = deviceBinding;
            Profile = profile;
            SubscriptionStateGuid = subscriptionStateGuid;
            DeviceBindingSubscriptionGuid = Guid.NewGuid();
            IsOverwritten = false;
            DeviceSubscription = new DeviceSubscription(GetDevice(), profile);
        }

        private Device GetDevice()
        {
            return Profile.GetDevice(DeviceBinding);
        }
    }
}
