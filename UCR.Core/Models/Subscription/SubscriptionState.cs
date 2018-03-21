using System;
using System.Collections.Generic;
using System.Linq;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class SubscriptionState
    {
        public Guid StateGuid { get; }
        public Profile ActiveProfile { get; }
        public bool IsActive { get; set; }

        public List<DeviceSubscription> OutputDeviceSubscriptions { get; }
        public List<MappingSubscription> MappingSubscriptions { get; set; }


        public SubscriptionState(Profile profile)
        {
            StateGuid = Guid.NewGuid();
            ActiveProfile = profile;
            OutputDeviceSubscriptions = new List<DeviceSubscription>();
            MappingSubscriptions = new List<MappingSubscription>();
            IsActive = false;
        }

        public DeviceSubscription AddOutputDevice(Device device, Profile profile)
        {
            var deviceSubscription = new DeviceSubscription(device, profile);
            OutputDeviceSubscriptions.Add(deviceSubscription);
            return deviceSubscription;
        }

        public void AddMapping(Mapping mapping, Profile profile, List<DeviceSubscription> profileOutputDevices)
        {
            MappingSubscriptions.Add(new MappingSubscription(profile, mapping, StateGuid, profileOutputDevices));
        }
    }
}
