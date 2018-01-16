using System.Collections.Generic;
using System.Linq;
using UCR.Core.Models.Binding;

namespace UCR.Core.Models.Subscription
{
    public class DeviceBindingSubscription
    {
        public DeviceBinding DeviceBinding { get; }
        public Profile.Profile Profile { get; }
        public bool IsOverwritten { get; set; }

        public DeviceBindingSubscription(DeviceBinding deviceBinding, Profile.Profile profile)
        {
            DeviceBinding = deviceBinding;
            Profile = profile;
            IsOverwritten = false;
        }

        public static List<DeviceBindingSubscription> GetSubscriptionsFromList(List<DeviceBinding> deviceBindings)
        {
            return deviceBindings.Select(deviceBinding => new DeviceBindingSubscription(deviceBinding, deviceBinding.Plugin.ParentProfile)).ToList();
        }

        public Device.Device GetLocalDevice()
        {
            return Profile.GetLocalDevice(DeviceBinding);
        }
    }
}
