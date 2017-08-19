using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Providers;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public enum KeyType
    {
        Button = 0,
        Axis = 1,
        Pov = 2
    }

    public sealed class Joystick : Device
    {
        // Persistence

        // Runtime
        // TODO Limits
        public int MaxButtons { get; set; }
        // axis array
        // pov
        
        // Subscriptions
        private Dictionary<string, DeviceBinding> Subscriptions;

        public Joystick() : base(DeviceType.Joystick)
        {
            ClearSubscribers();
        }

        public Joystick(Joystick joystick) : base(joystick)
        {
            MaxButtons = joystick.MaxButtons;
            ClearSubscribers();
        }

        public override bool AddDeviceBinding(DeviceBinding deviceBinding)
        {
            Subscriptions[deviceBinding.Plugin.Title] = deviceBinding;
            return true;
        }
        
        public override void ClearSubscribers()
        {
            Subscriptions = new Dictionary<string, DeviceBinding>();
        }

        public override void SubscribeDeviceBindings(UCRContext ctx)
        {
            foreach (var deviceBinding in Subscriptions)
            {
                SubscribeDeviceBindingInput(ctx, deviceBinding.Value);
            }
        }

        public override void SubscribeDeviceBindingInput(UCRContext ctx, DeviceBinding deviceBinding)
        {
            var success = ctx.IOController.SubscribeInput(new InputSubscriptionRequest()
            {
                InputType = MapDeviceBindingInputType(deviceBinding),
                Callback = deviceBinding.Callback,
                ProviderName = SubscriberProviderName,
                DeviceHandle = DeviceHandle,
                InputIndex = (uint)deviceBinding.KeyValue,
                SubscriberGuid = deviceBinding.Guid,
                ProfileGuid = deviceBinding.Plugin.ParentProfile.Guid
                //InputSubId = TODO
            });
        }

        protected override InputType MapDeviceBindingInputType(DeviceBinding deviceBinding)
        {
            switch ((KeyType)deviceBinding.KeyType)
            {
                case KeyType.Button:
                    return InputType.BUTTON;
                case KeyType.Axis:
                    return InputType.AXIS;
                case KeyType.Pov:
                    return InputType.POV;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
