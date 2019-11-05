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

        public MappingSubscription(Profile profile, Mapping mapping, Guid subscriptionStateGuid, List<DeviceSubscription> profileOutputDevices)
        {
            Mapping = mapping;
            Overriden = false;
            DeviceBindingSubscriptions = new List<InputSubscription>();
            foreach (var mappingDeviceBinding in Mapping.DeviceBindings)
            {
                DeviceBindingSubscriptions.Add(new InputSubscription(mappingDeviceBinding, profile, subscriptionStateGuid));
            }

            PluginSubscriptions = new List<PluginSubscription>();
            foreach (var mappingPlugin in Mapping.Plugins)
            {
                PluginSubscriptions.Add(new PluginSubscription(Mapping, mappingPlugin, subscriptionStateGuid, profileOutputDevices));
            }
        }
    }
}
