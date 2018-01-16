using System;
using System.Collections.Generic;
using System.Linq;
using UCR.Core.Models.Binding;

namespace UCR.Core.Models.Subscription
{
    public class SubscriptionState
    {
        public Profile.Profile ActiveProfile { get; }
        public Boolean IsActive { get; set; }

        public List<DeviceSubscription> DeviceSubscriptions { get; set; }
        public Dictionary<string, List<DeviceBindingSubscription>> DeviceBindingSubscriptions;
        public List<Plugin.Plugin> ActivePlugins;

        public SubscriptionState(Profile.Profile profile)
        {
            ActiveProfile = profile;
            DeviceSubscriptions = new List<DeviceSubscription>();
            DeviceBindingSubscriptions = new Dictionary<string, List<DeviceBindingSubscription>>();
            ActivePlugins = new List<Plugin.Plugin>();
            IsActive = false;
        }

        public void AddOutputDevice(Device.Device device, Profile.Profile profile)
        {
            DeviceSubscriptions.Add(new DeviceSubscription(device, profile));
        }

        public void AddDeviceBindingSubscriptions(Plugin.Plugin plugin)
        {
            if (DeviceBindingSubscriptions.ContainsKey(plugin.Title))
            {
                foreach (var deviceBindingSubscription in DeviceBindingSubscriptions[plugin.Title])
                {
                    deviceBindingSubscription.IsOverwritten = true;
                }
                DeviceBindingSubscriptions[plugin.Title].AddRange(DeviceBindingSubscription.GetSubscriptionsFromList(plugin.GetInputs()));
            }
            else
            {
                DeviceBindingSubscriptions[plugin.Title] = DeviceBindingSubscription.GetSubscriptionsFromList(plugin.GetInputs());
            }
        }

        public void BuildActivePluginsList()
        {
            foreach (var subscription in DeviceBindingSubscriptions)
            {
                ActivePlugins.Add(subscription.Value.First(d => d.IsOverwritten == false).DeviceBinding.Plugin);
            }
        }
    }
}
