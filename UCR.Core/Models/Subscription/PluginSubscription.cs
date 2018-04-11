using System;
using System.Collections.Generic;
using System.Linq;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class PluginSubscription
    {
        public Plugin Plugin { get; set; }
        public Guid SubscriptionStateGuid { get; set; }
        public List<OutputSubscription> OutputSubscriptions { get; set; }

        public PluginSubscription(Plugin plugin, Guid subscriptionStateGuid, List<DeviceSubscription> outputDeviceSubscriptions)
        {
            Plugin = plugin;
            SubscriptionStateGuid = subscriptionStateGuid;
            OutputSubscriptions = new List<OutputSubscription>();

            foreach (var deviceBinding in Plugin.Outputs)
            {
                if (!deviceBinding.IsBound) continue;
                var outputDeviceSubscription = outputDeviceSubscriptions.FirstOrDefault(d => d.Device.Guid == deviceBinding.DeviceGuid);
                OutputSubscriptions.Add(new OutputSubscription(deviceBinding, subscriptionStateGuid, outputDeviceSubscription));
            }
        }
    }
}
