using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using UCR.Models.Devices;
using UCR.Models.Mapping;
using UCR.Views.Controls;
using Binding = UCR.Models.Mapping.Binding;

namespace UCR.Models.Plugins
{
    public abstract class Plugin
    {
        // Persistence
        public String Title { get; set; }
        private Profile Profile { get; set; }
        public List<Binding> Inputs { get; set; } // TODO Private
        public List<Binding> Outputs { get; set; } // TODO Private

        // Runtime
        public DataTemplate PluginTemplate { get; set; }

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
            Console.WriteLine("Input button pressed on devicetype " + output.DeviceType + " Value:" + value);
            if (value != 0) SendKeys.SendWait(""+output.KeyValue);
            if (output?.DeviceType == null || output?.KeyValue == null) return;
            
            return; // TODO remove;
            // TODO Implement
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
                input.PluginName = Title;
                if (input.DeviceType == null) continue;
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
