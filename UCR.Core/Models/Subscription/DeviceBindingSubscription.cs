using System;
using System.Collections.Generic;
using System.Linq;
using UCR.Core.Managers;
using UCR.Core.Models.Binding;

namespace UCR.Core.Models.Subscription
{
    public class DeviceBindingSubscription
    {
        public DeviceBinding DeviceBinding { get; }
        public Profile.Profile Profile { get; }
        public Guid SubscriptionStateGuid { get; set; }
        public Guid DeviceBindingSubscriptionGuid { get; set; }
        public bool IsOverwritten { get; set; }
        public DeviceSubscription DeviceSubscription { get; }

        public DeviceBindingSubscription(DeviceBinding deviceBinding, Profile.Profile profile, Guid subscriptionStateGuid, List<DeviceSubscription> deviceSubscriptions)
        {
            DeviceBinding = deviceBinding;
            deviceBinding.OutputSink = WriteOutput;
            Profile = profile;
            SubscriptionStateGuid = subscriptionStateGuid;
            DeviceBindingSubscriptionGuid = Guid.NewGuid();
            IsOverwritten = false;

            var device = GetDevice();
            DeviceSubscription = deviceSubscriptions.Find(ds => ds.Device.Guid == device.Guid) ?? new DeviceSubscription(device, profile);
        }

        public static List<DeviceBindingSubscription> GetSubscriptionsFromList(List<DeviceBinding> deviceBindings, Guid subscriptionStateGuid, List<DeviceSubscription> deviceSubscriptions)
        {
            return deviceBindings.Select(deviceBinding => new DeviceBindingSubscription(deviceBinding, deviceBinding.Plugin.ParentProfile, subscriptionStateGuid, deviceSubscriptions)).ToList();
        }

        private Device.Device GetDevice()
        {
            return Profile.GetDevice(DeviceBinding);
        }

        public void WriteOutput(long value)
        {
            // TODO get context properly
            var success = Profile.context.IOController.SetOutputstate(SubscriptionsManager.GetOutputSubscriptionRequest(SubscriptionStateGuid, DeviceSubscription), SubscriptionsManager.GetBindingDescriptor(DeviceBinding), (int)value);
            var a = 1;
        }
    }
}
