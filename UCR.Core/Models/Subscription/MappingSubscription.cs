using System;
using System.Collections.Generic;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class MappingSubscription
    {
        public Mapping Mapping { get; }
        public List<InputSubscription> DeviceBindingSubscriptions { get; }
        public List<PluginSubscription> PluginSubscriptions { get; }
        public bool Overriden { get; set; }

        public MappingSubscription(Profile profile, Mapping mapping, Guid subscriptionStateGuid, List<DeviceConfigurationSubscription> subscriptionOutputDeviceConfigurations)
        {
            Mapping = mapping;
            Overriden = false;
            DeviceBindingSubscriptions = new List<InputSubscription>();
            foreach (var mappingDeviceBinding in Mapping.DeviceBindings)
            {
                if (!mappingDeviceBinding.IsBound) continue;

                var inputSubscription = new InputSubscription(mapping, mappingDeviceBinding, profile, subscriptionStateGuid);
                if (inputSubscription.DeviceSubscription != null) DeviceBindingSubscriptions.Add(inputSubscription);
            }

            PluginSubscriptions = new List<PluginSubscription>();
            foreach (var mappingPlugin in Mapping.Plugins)
            {
                PluginSubscriptions.Add(new PluginSubscription(Mapping, mappingPlugin, subscriptionStateGuid, subscriptionOutputDeviceConfigurations));
            }
        }
    }
}
