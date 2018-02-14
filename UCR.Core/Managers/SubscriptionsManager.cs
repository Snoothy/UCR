using System;
using NLog;
using IProvider;
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

        public Profile GetActiveProfile()
        {
            return SubscriptionState?.ActiveProfile;
        }

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
            Logger.Debug("Successfully populated subscription state");

            if (!ActivateSubscriptionState(state))
            {
                Logger.Error("Failed to activate profile successfully");
                return false;
            }
            Logger.Debug("SubscriptionState successfully activated");

            if (!DeactivateProfile()) Logger.Error("Failed to deactivate previous profile successfully");

            _context.IOController.SetProfileState(state.StateGuid, true);

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

            foreach (var deviceBindingSubscriptionsGroup in state.DeviceBindingSubscriptions)
            {
                foreach (var subscription in deviceBindingSubscriptionsGroup.Value)
                {
                    if (subscription.IsOverwritten) continue;
                    success &= UnsubscribeDeviceBindingInput(state, subscription);
                }
            }

            foreach (var deviceSubscription in state.DeviceSubscriptions)
            {
                success &= UnsubscribeOutput(state, deviceSubscription);
            }

            foreach (var plugin in state.ActivePlugins)
            {
                plugin.OnDeactivate();
            }

            _context.IOController.SetProfileState(state.StateGuid, false);

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

            foreach (var deviceSubscription in state.DeviceSubscriptions)
            {
                success &= SubscribeOutput(state, deviceSubscription);
            }

            foreach (var deviceBindingSubscriptionsGroup in state.DeviceBindingSubscriptions)
            {
                foreach (var subscription in deviceBindingSubscriptionsGroup.Value)
                {
                    if (subscription.IsOverwritten) continue;
                    success &= SubscribeDeviceBindingInput(state, subscription);
                }
            }

            foreach (var plugin in state.ActivePlugins)
            {
                plugin.OnActivate();
            }

            state.IsActive = true;
            return success;
        }

        private bool SubscribeDeviceBindingInput(SubscriptionState state, DeviceBindingSubscription deviceBindingSubscription)
        {
            return deviceBindingSubscription.DeviceBinding.IsBound
                ? _context.IOController.SubscribeInput(GetInputSubscriptionRequest(state, deviceBindingSubscription))
                : UnsubscribeDeviceBindingInput(state, deviceBindingSubscription);
        }

        private bool UnsubscribeDeviceBindingInput(SubscriptionState state, DeviceBindingSubscription deviceBindingSubscription)
        {
            return _context.IOController.UnsubscribeInput(GetInputSubscriptionRequest(state, deviceBindingSubscription));
        }

        private bool SubscribeOutput(SubscriptionState state, DeviceSubscription deviceSubscription)
        {
            Logger.Debug($"Subscribing output device: {{{deviceSubscription.Device.LogName()}}}");
            if (string.IsNullOrEmpty(deviceSubscription.Device.ProviderName) || string.IsNullOrEmpty(deviceSubscription.Device.DeviceHandle))
            {
                Logger.Error($"Failed to subscribe output device. Providername or devicehandle missing from: {{{deviceSubscription.Device.LogName()}}}");
                return false;
            }
            return _context.IOController.SubscribeOutput(GetOutputSubscriptionRequest(state.StateGuid, deviceSubscription));
        }

        private bool UnsubscribeOutput(SubscriptionState state, DeviceSubscription deviceSubscription)
        {
            Logger.Debug($"Unsubscribing output device: {{{deviceSubscription.Device.LogName()}}}");
            if (string.IsNullOrEmpty(deviceSubscription.Device.ProviderName) || string.IsNullOrEmpty(deviceSubscription.Device.DeviceHandle))
            {
                Logger.Error($"Failed to unsubscribe output device. Providername or devicehandle missing from: {{{deviceSubscription.Device.LogName()}}}");
                return false;
            }
            return _context.IOController.UnsubscribeOutput(GetOutputSubscriptionRequest(state.StateGuid, deviceSubscription));
        }

        #region DescriptionHelpers

        private InputSubscriptionRequest GetInputSubscriptionRequest(SubscriptionState state, DeviceBindingSubscription deviceBindingSubscription)
        {
            var device = deviceBindingSubscription.DeviceSubscription.Device;
            return new InputSubscriptionRequest()
            {
                ProviderDescriptor = GetProviderDescriptor(device),
                DeviceDescriptor = GetDeviceDescriptor(device),
                SubscriptionDescriptor = GetSubscriptionDescriptor(deviceBindingSubscription.DeviceBindingSubscriptionGuid, state.StateGuid),
                BindingDescriptor = GetBindingDescriptor(deviceBindingSubscription.DeviceBinding),
                Callback = deviceBindingSubscription.DeviceBinding.Callback
            };
        }

        public static OutputSubscriptionRequest GetOutputSubscriptionRequest(Guid subscriptionStateGuid, DeviceSubscription deviceSubscription)
        {
            return new OutputSubscriptionRequest()
            {
                ProviderDescriptor = GetProviderDescriptor(deviceSubscription.Device),
                DeviceDescriptor = GetDeviceDescriptor(deviceSubscription.Device),
                SubscriptionDescriptor = GetSubscriptionDescriptor(deviceSubscription.DeviceSubscriptionGuid, subscriptionStateGuid)
            };
        }

        private static ProviderDescriptor GetProviderDescriptor(Device device)
        {
            return new ProviderDescriptor()
            {
                ProviderName = device.ProviderName
            };
        }

        private static DeviceDescriptor GetDeviceDescriptor(Device device)
        {
            return new DeviceDescriptor()
            {
                DeviceHandle = device.DeviceHandle,
                DeviceInstance = device.DeviceNumber
            };
        }

        private static SubscriptionDescriptor GetSubscriptionDescriptor(Guid subscriberGuid, Guid profileGuid)
        {
            return new SubscriptionDescriptor()
            {
                SubscriberGuid = subscriberGuid,
                ProfileGuid = profileGuid
            };
        }

        public static BindingDescriptor GetBindingDescriptor(DeviceBinding deviceBinding)
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
