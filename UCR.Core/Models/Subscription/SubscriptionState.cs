using System;
using System.Collections.Generic;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class SubscriptionState
    {
        public Guid StateGuid { get; }
        public Profile ActiveProfile { get; }
        public bool IsActive { get; set; }

        public List<DeviceConfigurationSubscription> OutputDeviceConfigurationSubscriptions { get; }
        public List<MappingSubscription> MappingSubscriptions { get; set; }


        public SubscriptionState(Profile profile)
        {
            StateGuid = Guid.NewGuid();
            ActiveProfile = profile;
            OutputDeviceConfigurationSubscriptions = new List<DeviceConfigurationSubscription>();
            MappingSubscriptions = new List<MappingSubscription>();
            IsActive = false;
        }

        public void AddOutputDeviceConfiguration(DeviceConfiguration deviceConfiguration)
        {
            var deviceSubscription = new DeviceConfigurationSubscription(deviceConfiguration);
            OutputDeviceConfigurationSubscriptions.Add(deviceSubscription);
        }
        
        public void AddMappings(Profile profile, List<DeviceConfigurationSubscription> profileOutputDevices)
        {
            var profileMappings = new List<MappingSubscription>();

            foreach (var profileMapping in profile.Mappings)
            {
                profileMappings.Add(new MappingSubscription(profile, profileMapping, StateGuid, profileOutputDevices));
            }

            OverrideParentMappings(profileMappings);

            MappingSubscriptions.AddRange(profileMappings);
            MappingSubscriptions.AddRange(AddShadowMappings(profileMappings));
        }

        private List<MappingSubscription> AddShadowMappings(List<MappingSubscription> profileMappings)
        {
            // TODO Implement for shadow devices
            return new List<MappingSubscription>();
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
