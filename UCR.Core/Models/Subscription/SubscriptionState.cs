using System;
using System.Collections.Concurrent;
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
        public FilterState FilterState { get; set; }

        public SubscriptionState(Profile profile)
        {
            StateGuid = Guid.NewGuid();
            ActiveProfile = profile;
            OutputDeviceConfigurationSubscriptions = new List<DeviceConfigurationSubscription>();
            MappingSubscriptions = new List<MappingSubscription>();
            IsActive = false;

            FilterState = new FilterState();
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
            MappingSubscriptions.AddRange(AddShadowMappings(profile, profileMappings, profileOutputDevices));
        }

        private List<MappingSubscription> AddShadowMappings(Profile profile, List<MappingSubscription> profileMappings, List<DeviceConfigurationSubscription> profileOutputDevices)
        {
            var result = new List<MappingSubscription>();

            foreach (var mappingSubscription in profileMappings)
            {
                var shadowClones = mappingSubscription.Mapping.PossibleShadowClones;
                if (shadowClones == 0) continue;
                

                result.AddRange(CloneMappingSubscription(profile, mappingSubscription, profileOutputDevices, shadowClones));
            }

            return result;
        }

        private List<MappingSubscription> CloneMappingSubscription(Profile profile, MappingSubscription mappingSubscription, List<DeviceConfigurationSubscription> profileOutputDevices, int shadowClones)
        {
            var result = new List<MappingSubscription>();

            for (var i = 0; i < shadowClones; i++)
            {
                result.Add(new MappingSubscription(profile, mappingSubscription.Mapping.CreateShadowClone(i), StateGuid, profileOutputDevices));
            }

            return result;
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
