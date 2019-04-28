
using System;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models.Subscription
{
    public class OutputSubscription
    {
        public DeviceBinding DeviceBinding { get; }
        public Guid SubscriptionStateGuid { get; }
        public DeviceSubscription DeviceSubscription { get; }

        public OutputSubscription(DeviceBinding deviceBinding, Guid subscriptionStateGuid, DeviceSubscription outputDeviceSubscription)
        {
            DeviceBinding = deviceBinding;
            SubscriptionStateGuid = subscriptionStateGuid;
            DeviceSubscription = outputDeviceSubscription;
            deviceBinding.OutputSink = WriteOutput;
        }


        private void WriteOutput(short value)
        {
            DeviceBinding.Profile.Context.IOController.SetOutputstate(SubscriptionsManager.GetOutputSubscriptionRequest(SubscriptionStateGuid, DeviceSubscription), SubscriptionsManager.GetBindingDescriptor(DeviceBinding), (int)value);
        }
    }
}
