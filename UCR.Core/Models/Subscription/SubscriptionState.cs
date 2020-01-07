using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

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
        public List<Plugin> FixedUpdatePlugins { get; set; }
        
        // Fixed update
        private Stopwatch Stopwatch { get; }
        public CancellationTokenSource CancellationTokenSource { get; set; }
        private bool HasFixedUpdatePlugins => FixedUpdatePlugins.Count > 0;

        public SubscriptionState(Profile profile)
        {
            StateGuid = Guid.NewGuid();
            ActiveProfile = profile;
            OutputDeviceConfigurationSubscriptions = new List<DeviceConfigurationSubscription>();
            MappingSubscriptions = new List<MappingSubscription>();
            IsActive = false;

            FilterState = new FilterState();
            FixedUpdatePlugins = new List<Plugin>();
            Stopwatch = new Stopwatch();
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

            RegisterFixedUpdatePlugins();
        }

        public void Activate()
        {
            if (HasFixedUpdatePlugins)
            {
                CancellationTokenSource = new CancellationTokenSource();
                var task = Task.Factory.StartNew(UpdatePlugins, CancellationTokenSource.Token);
            }

            IsActive = true;
        }

        private void UpdatePlugins()
        {
            Stopwatch.Start();
            var lastUpdate = Stopwatch.ElapsedMilliseconds;
            long delta = 8;

            while (!CancellationTokenSource.IsCancellationRequested)
            {
                if (Stopwatch.ElapsedMilliseconds - lastUpdate < 8)
                {
                    Thread.Sleep(4);
                    continue;
                }

                delta = Stopwatch.ElapsedMilliseconds - lastUpdate;

                foreach (var plugin in FixedUpdatePlugins)
                {
                    plugin.FixedUpdate(delta);
                }

                
                lastUpdate = Stopwatch.ElapsedMilliseconds;
            }
        }

        public void Deactivate()
        {
            if (HasFixedUpdatePlugins)
            {
                Stopwatch.Reset();
                CancellationTokenSource.Cancel();
            }

            IsActive = false;
        }

        private void RegisterFixedUpdatePlugins()
        {
            foreach (var mappingSubscription in MappingSubscriptions)
            {
                if (mappingSubscription.Overriden) continue;

                foreach (var pluginSubscription in mappingSubscription.PluginSubscriptions)
                {
                    if (!pluginSubscription.Plugin.HasFixedUpdate) continue;

                    FixedUpdatePlugins.Add(pluginSubscription.Plugin);
                }
            }
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
