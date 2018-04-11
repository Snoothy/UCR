using System;
using System.Collections.Generic;

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
        
        public void AddMappings(Profile profile, List<DeviceSubscription> profileOutputDevices)
        {
            var profileMappings = new List<MappingSubscription>();

            foreach (var profileMapping in profile.Mappings)
            {
                profileMappings.Add(new MappingSubscription(profile, profileMapping, StateGuid, profileOutputDevices));
            }

            OverrideParentMappings(profileMappings);
            MappingSubscriptions.AddRange(profileMappings);
        }

        private void OverrideParentMappings(List<MappingSubscription> profileMappingSubscriptions)
        {
            foreach (var profileMappingSubscription in profileMappingSubscriptions)
            {
                foreach (var subscription in MappingSubscriptions)
                {
                    if (profileMappingSubscription.Mapping.Title.Equals(subscription.Mapping.Title))
                    {
                        subscription.Overriden = true;
                    }
                }
            }
        }
    }
}
