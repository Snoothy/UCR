using System;
using System.Collections.Generic;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class MappingSubscription
    {
        public Mapping Mapping { get; set; }
        public List<DeviceBindingSubscription> DeviceBindingSubscriptions { get; set; }
        public List<PluginSubscription> PluginSubscriptions { get; set; }

        public MappingSubscription(Profile profile, Mapping mapping, Guid subscriptionStateGuid, List<DeviceSubscription> profileOutputDevices)
        {
            Mapping = mapping;
            DeviceBindingSubscriptions = new List<DeviceBindingSubscription>();
            foreach (var mappingDeviceBinding in Mapping.DeviceBindings)
            {
                DeviceBindingSubscriptions.Add(new DeviceBindingSubscription(mappingDeviceBinding, profile, subscriptionStateGuid));
            }

            PluginSubscriptions = new List<PluginSubscription>();
            foreach (var mappingPlugin in Mapping.Plugins)
            {
                if (!mappingPlugin.Output.IsBound) continue;
                PluginSubscriptions.Add(new PluginSubscription(mappingPlugin, subscriptionStateGuid, profileOutputDevices));
            }
        }
    }
}
