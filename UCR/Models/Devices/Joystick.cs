using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
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
        private Dictionary<int, Dictionary<String, DeviceBinding.ValueChanged>> ButtonCallbacks;
        private Dictionary<int, Dictionary<String, DeviceBinding.ValueChanged>> AxisCallbacks;
        private Dictionary<int, Dictionary<String, DeviceBinding.ValueChanged>> PovCallbacks;

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

        public override bool Subscribe(DeviceBinding deviceBinding)
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

        private void AddSubscriber(Dictionary<int, Dictionary<String, DeviceBinding.ValueChanged>> keylist, DeviceBinding deviceBinding)
        {
            if (!keylist.ContainsKey(deviceBinding.KeyValue))
            {
                keylist.Add(deviceBinding.KeyValue, new Dictionary<String, DeviceBinding.ValueChanged>());
            }

            var subscriberlist = keylist[deviceBinding.KeyValue];
            subscriberlist[deviceBinding.PluginName] = deviceBinding.Callback;
        }

        public override void ClearSubscribers()
        {
            ButtonCallbacks = new Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>>();
            AxisCallbacks = new Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>>();
            PovCallbacks = new Dictionary<int, Dictionary<string, DeviceBinding.ValueChanged>>();
        }

        public override void Activate(UCRContext ctx)
        {
            // Subscribe buttons
            foreach (var buttonCallback in ButtonCallbacks)
            {
                foreach (var binding in buttonCallback.Value)
                {
                    // TODO Save guid for unsubscription
                    var SubGuid = ctx.IOController.SubscribeButton(SubscriberPluginName, Guid, (uint) buttonCallback.Key,
                        new Action<long>((value) => binding.Value(value)));
                }
            }
            // TODO Bind to IOWrapper
            //throw new NotImplementedException();
        }
    }
}
