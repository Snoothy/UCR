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
        private Dictionary<int, Dictionary<String, Binding.ValueChanged>> ButtonCallbacks;
        private Dictionary<int, Dictionary<String, Binding.ValueChanged>> AxisCallbacks;
        private Dictionary<int, Dictionary<String, Binding.ValueChanged>> PovCallbacks;

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

        public override bool Subscribe(Binding binding)
        {
            switch ((KeyType)binding.KeyType)
            {
                case KeyType.Button:
                    AddSubscriber(ButtonCallbacks, binding);
                    break;
                case KeyType.Axis:
                    AddSubscriber(AxisCallbacks, binding);
                    break;
                case KeyType.Pov:
                    AddSubscriber(PovCallbacks, binding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }

        private void AddSubscriber(Dictionary<int, Dictionary<String, Binding.ValueChanged>> keylist, Binding binding)
        {
            if (!keylist.ContainsKey(binding.KeyValue))
            {
                keylist.Add(binding.KeyValue, new Dictionary<String, Binding.ValueChanged>());
            }

            var subscriberlist = keylist[binding.KeyValue];
            subscriberlist[binding.PluginName] = binding.Callback;
        }

        public override void ClearSubscribers()
        {
            ButtonCallbacks = new Dictionary<int, Dictionary<string, Binding.ValueChanged>>();
            AxisCallbacks = new Dictionary<int, Dictionary<string, Binding.ValueChanged>>();
            PovCallbacks = new Dictionary<int, Dictionary<string, Binding.ValueChanged>>();
        }

        public override void Activate()
        {
            int a = 0;
            // TODO Bind to IOWrapper
            //throw new NotImplementedException();
        }

        public void Test()
        {
            foreach (var buttonCallback in ButtonCallbacks)
            {
                foreach (var valueChanged in buttonCallback.Value)
                {
                    valueChanged.Value(10);
                }
            }
        }
    }
}
