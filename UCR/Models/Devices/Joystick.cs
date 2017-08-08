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
    public enum InputType
    {
        DirectInput,
        XInput
    }

    public enum KeyType
    {
        Button = 0,
        Axis = 1,
        Pov = 2
    }

    public sealed class Joystick : Device
    {
        // Persistence
        public InputType InputType { get; }

        // TODO Limits
        public int MaxButtons { get; set; }
        // axis array
        // pov

        // Runtime
        // Subscriptions
        private Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>> ButtonCallbacks;
        private Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>> AxisCallbacks;
        private Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>> PovCallbacks;

        public Joystick(InputType inputType) : base(DeviceType.Joystick)
        {
            InputType = inputType;
            ClearSubscribers();
        }

        public Joystick()
        {

        }

        public Joystick(Joystick joystick) : base(joystick)
        {
            InputType = joystick.InputType;
            ClearSubscribers();
        }

        public override bool SubscribeInput(DeviceBinding deviceBinding)
        {
            switch ((KeyType)deviceBinding.KeyType)
            {
                case KeyType.Button:
                    AddSubscriber(ButtonCallbacks, deviceBinding);
                    break;
                case KeyType.Axis:
                    AddSubscriber(AxisCallbacks, deviceBinding);
                    break;
                case KeyType.Pov:
                    AddSubscriber(PovCallbacks, deviceBinding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        public override bool SubscribeOutput(DeviceBinding deviceBinding)
        {
            throw new NotImplementedException();
        }

        private void AddSubscriber(Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>> keylist, DeviceBinding deviceBinding)
        {
            if (!keylist.ContainsKey(deviceBinding.KeyValue))
            {
                keylist.Add(deviceBinding.KeyValue, new Dictionary<string, DeviceBinding.ValueChanged>());
            }

            var subscriberlist = keylist[deviceBinding.KeyValue];
            subscriberlist[deviceBinding.Plugin.Title] = deviceBinding.Callback;
        }

        public override void ClearSubscribers()
        {
            ButtonCallbacks = new Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>>();
            AxisCallbacks = new Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>>();
            PovCallbacks = new Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>>();
        }

        public override void SubscribeDeviceBindings(UCRContext ctx)
        {
            // Subscribe buttons
            foreach (var buttonCallback in ButtonCallbacks)
            {
                foreach (var binding in buttonCallback.Value)
                {
                   // TODO Save guid for unsubscription
                    var SubGuid = ctx.IOController.SubscribeInput(new InputSubscriptionRequest()
                    {
                        InputType = Providers.InputType.BUTTON,
                        Callback = binding.Value,
                        ProviderName = SubscriberProviderName,
                        DeviceHandle = Guid,
                        InputIndex = (uint) buttonCallback.Key,
                        SubscriberGuid = new Guid() // TODO Save on devicebinding

                    });
                }
            }
            // TODO Bind to IOWrapper
            //throw new NotImplementedException();
        }
    }
}
