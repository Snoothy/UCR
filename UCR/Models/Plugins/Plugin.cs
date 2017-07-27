using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Devices;
using UCR.Models.Mapping;

namespace UCR.Models.Plugins
{
    public abstract class Plugin
    {
        public String Title { get; set; }
        private Profile Profile { get; set; }
        private List<Binding> Inputs { get; set; }
        private List<Binding> Outputs { get; set; }

        public Plugin(Profile profile)
        {
            Profile = profile;
            Inputs = new List<Binding>();
            Outputs = new List<Binding>();
        }

        public bool Activate(UCRContext ctx)
        {
            bool success = true;
            success &= SubscribeInputs(ctx);
            return success;
        }

        protected void WriteOutput(Binding output, long value)
        {
            if (output?.DeviceType == null || output?.KeyValue == null) return;
            switch (output?.DeviceType)
            {
                case DeviceType.Keyboard:
                    throw new NotImplementedException();
                    break;
                case DeviceType.Mouse:
                    throw new NotImplementedException();
                    break;
                case DeviceType.Joystick:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool SubscribeInputs(UCRContext ctx)
        {
            bool success = true;
            foreach (var input in Inputs)
            {
                var device = Profile.GetDevice(input);
                if (device != null)
                {
                    // TODO test if switch is needed (type erasure?)
                    switch (input.DeviceType)
                    {
                        case DeviceType.Keyboard:
                            success &= ((Keyboard) device).Subscribe(input);
                            break;
                        case DeviceType.Mouse:
                            success &= ((Mouse)device).Subscribe(input);
                            break;
                        case DeviceType.Joystick:
                            success &= ((Joystick)device).Subscribe(input);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }

        protected Binding InitializeInputMapping(Binding.ValueChanged callbackFunc)
        {
            return InitializeMapping(BindingType.Input, callbackFunc);
        }

        protected Binding InitializeOutputMapping()
        {
            return InitializeMapping(BindingType.Output, null);
        }

        private Binding InitializeMapping(BindingType bindingType, Binding.ValueChanged callbackFunc)
        {
            Binding binding = new Binding(callbackFunc);
            switch(bindingType)
            {
                case BindingType.Input:
                    Inputs.Add(binding);
                    break;
                case BindingType.Output:
                    Outputs.Add(binding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bindingType), bindingType, null);
            }
            return binding;
        }
    }
}
