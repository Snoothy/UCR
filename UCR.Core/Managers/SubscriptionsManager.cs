using System;
using NLog;
using Providers;
using UCR.Core.Models.Binding;
using UCR.Core.Models.Device;
using UCR.Core.Models.Profile;
using UCR.Core.Models.Subscription;
using Logger = NLog.Logger;

namespace UCR.Core.Managers
{
    public class SubscriptionsManager : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private SubscriptionState SubscriptionState { get; set; }
        private readonly Context _context;

        public SubscriptionsManager(Context context)
        {
            _context = context;
        }

        #region ManagerApi

        public bool ActivateProfile(Profile profile)
        {
            Logger.Debug($"Activating profile: {{{profile.ProfileBreadCrumbs()}}}");
            if (SubscriptionState?.ActiveProfile?.Guid == profile.Guid) return true;

            var state = new SubscriptionState(profile);
            if (!PopulateSubscriptionStateForProfile(state, profile))
            {
                Logger.Error("Failed to populate SubscriptionState");
                return false;
            }
            Logger.Debug($"Successfully subscribed profile");

            if (!ActivateSubscriptionState(state)) return false;
            Logger.Debug("SubscriptionState successfully activated");

            if (!DeactivateProfile()) Logger.Error("Failed to deactivate previous profile successfully");

            SubscriptionState = state;
            _context.ActiveProfile = profile;
            foreach (var action in _context.ActiveProfileCallbacks)
            {
                action();
            }
            
            return true;
        }

        public bool DeactivateProfile()
        {
            if (SubscriptionState == null) return true;
            var success = true;
            var state = SubscriptionState;

            if (!state.IsActive) return true;
            var profiles = state.ActiveProfile.GetAncestry();
            foreach (var profile in profiles)
            {
                success &= _context.IOController.SetProfileState(profile.Guid, false);
            }

            foreach (var deviceBindingSubscriptionsGroup in state.DeviceBindingSubscriptions)
            {
                foreach (var subscription in deviceBindingSubscriptionsGroup.Value)
                {
                    if (subscription.IsOverwritten) continue;
                    success &= UnsubscribeDeviceBindingInput(subscription);
                }
            }

            foreach (var deviceSubscription in state.DeviceSubscriptions)
            {
                success &= UnsubscribeOutput(deviceSubscription.Device);
            }

            foreach (var plugin in state.ActivePlugins)
            {
                plugin.OnDeactivate();
            }

            SubscriptionState = null;
            _context.ActiveProfile = null;
            foreach (var action in _context.ActiveProfileCallbacks)
            {
                action();
            }

            return success;
        }

        #endregion

        private bool PopulateSubscriptionStateForProfile(SubscriptionState state, Profile profile)
        {
            var success = true;
            if (profile.ParentProfile != null)
            {
                success &= PopulateSubscriptionStateForProfile(state, profile.ParentProfile);
            }

            // Output devices
            var outputDeviceGroup = _context.DeviceGroupsManager.GetDeviceGroup(DeviceIoType.Output, profile.OutputDeviceGroupGuid);
            foreach (var device in outputDeviceGroup.Devices)
            {
                state.AddOutputDevice(device, profile);
            }

            // InputSubscriptions
            foreach (var profilePlugin in profile.Plugins)
            {
                state.AddDeviceBindingSubscriptions(profilePlugin);
            }

            state.BuildActivePluginsList();

            return success;
        }

        
        // Subscribes the backend when it is built
        private bool ActivateSubscriptionState(SubscriptionState state)
        {
            var success = true;
            if (state.IsActive) return true;
            var profiles = state.ActiveProfile.GetAncestry();
            foreach (var profile in profiles)
            {
                profile.Activate();
            }

            foreach (var deviceSubscription in state.DeviceSubscriptions)
            {
                deviceSubscription.Device.Reset(deviceSubscription.Profile);
                success &= SubscribeOutput(deviceSubscription.Device);
            }

            foreach (var deviceBindingSubscriptionsGroup in state.DeviceBindingSubscriptions)
            {
                foreach (var subscription in deviceBindingSubscriptionsGroup.Value)
                {
                    if (subscription.IsOverwritten) continue;
                    success &= SubscribeDeviceBindingInput(subscription);
                }
            }

            foreach (var profile in profiles)
            {
                success &= _context.IOController.SetProfileState(profile.Guid, true);
            }

            foreach (var plugin in state.ActivePlugins)
            {
                plugin.OnActivate();
            }

            state.IsActive = true;
            return success;
        }

        private bool SubscribeDeviceBindingInput(DeviceBindingSubscription deviceBindingSubscription)
        {
            return deviceBindingSubscription.DeviceBinding.IsBound
                ? _context.IOController.SubscribeInput(GetInputSubscriptionRequest(deviceBindingSubscription))
                : UnsubscribeDeviceBindingInput(deviceBindingSubscription);
        }

        private bool UnsubscribeDeviceBindingInput(DeviceBindingSubscription deviceBindingSubscription)
        {
            return _context.IOController.UnsubscribeInput(GetInputSubscriptionRequest(deviceBindingSubscription));
        }

        public bool SubscribeOutput(Device device)
        {
            Logger.Debug($"Subscribing output device: {{{device.LogName()}}}");
            if (string.IsNullOrEmpty(device.ProviderName) || string.IsNullOrEmpty(device.DeviceHandle))
            {
                Logger.Error($"Failed to subscribe output device. Providername or devicehandle missing from: {{{device.LogName()}}}");
                return false;
            }
            if (device.IsAcquired)
            {
                Logger.Debug("Device already acquired");
                return true;
            }
            device.IsAcquired = true;
            return _context.IOController.SubscribeOutput(GetOutputSubscriptionRequest(device));
        }

        public bool UnsubscribeOutput(Device device)
        {
            Logger.Debug($"Unsubscribing output device: {{{device.LogName()}}}");
            if (string.IsNullOrEmpty(device.ProviderName) || string.IsNullOrEmpty(device.DeviceHandle))
            {
                Logger.Error($"Failed to unsubscribe output device. Providername or devicehandle missing from: {{{device.LogName()}}}");
                return false;
            }
            if (!device.IsAcquired)
            {
                Logger.Debug("Device already unacquired");
                return true;
            }
            device.IsAcquired = false;
            return _context.IOController.UnsubscribeOutput(GetOutputSubscriptionRequest(device));
        }

        #region DescriptionHelpers

        private InputSubscriptionRequest GetInputSubscriptionRequest(DeviceBindingSubscription deviceBindingSubscription)
        {
            var device = deviceBindingSubscription.GetLocalDevice();
            return new InputSubscriptionRequest()
            {
                ProviderDescriptor = GetProviderDescriptor(device),
                DeviceDescriptor = GetDeviceDescriptor(device),
                SubscriptionDescriptor = GetSubscriptionDescriptor(deviceBindingSubscription.DeviceBinding.Guid, deviceBindingSubscription.DeviceBinding.Plugin.ParentProfile.Guid),
                BindingDescriptor = GetBindingDescriptor(deviceBindingSubscription.DeviceBinding),
                Callback = deviceBindingSubscription.DeviceBinding.Callback
            };
        }

        private OutputSubscriptionRequest GetOutputSubscriptionRequest(Device device)
        {
            return new OutputSubscriptionRequest()
            {
                ProviderDescriptor = GetProviderDescriptor(device),
                DeviceDescriptor = GetDeviceDescriptor(device),
                SubscriptionDescriptor = GetSubscriptionDescriptor(device.Guid, device.ParentProfile.Guid)
            };
        }

        private ProviderDescriptor GetProviderDescriptor(Device device)
        {
            return new ProviderDescriptor()
            {
                ProviderName = device.ProviderName
            };
        }

        private DeviceDescriptor GetDeviceDescriptor(Device device)
        {
            return new DeviceDescriptor()
            {
                DeviceHandle = device.DeviceHandle,
                DeviceInstance = device.DeviceNumber
            };
        }

        private SubscriptionDescriptor GetSubscriptionDescriptor(Guid subscriberGuid, Guid profileGuid)
        {
            return new SubscriptionDescriptor()
            {
                SubscriberGuid = subscriberGuid,
                ProfileGuid = profileGuid
            };
        }

        private BindingDescriptor GetBindingDescriptor(DeviceBinding deviceBinding)
        {
            return new BindingDescriptor()
            {
                Type = (BindingType)deviceBinding.KeyType,
                Index = deviceBinding.KeyValue,
                SubIndex = deviceBinding.KeySubValue
            };
        }

        #endregion

        public void Dispose()
        {
            if (SubscriptionState != null)
            {
                DeactivateProfile();
            }
        }
    }
}
