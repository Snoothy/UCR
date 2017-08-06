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

namespace UCR.Models.Plugins
{
    public abstract class Plugin
    {
        // Persistence
        public String Title { get; set; }
        private Profile Profile { get; set; }
        public List<DeviceBinding> Inputs { get; set; } // TODO Private
        public List<DeviceBinding> Outputs { get; set; } // TODO Private

        // Runtime
        public String PluginName { get; set; }

        public Plugin()
        {
            PluginName = "ButtonToButton";
        }

        public Plugin(Profile profile) : base()
        {
            Profile = profile;
            Inputs = new List<DeviceBinding>();
            Outputs = new List<DeviceBinding>();
        }

        public bool Activate(UCRContext ctx)
        {
            bool success = true;
            success &= SubscribeInputs(ctx);
            return success;
        }

        protected void WriteOutput(DeviceBinding output, long value)
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

        public virtual List<DeviceBinding> GetInputs()
        {
            return Inputs.Select(input => new DeviceBinding(input)).ToList();
        }

        private bool SubscribeInputs(UCRContext ctx)
        {
            bool success = true;
            foreach (var input in GetInputs())
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

        protected DeviceBinding InitializeInputMapping(DeviceBinding.ValueChanged callbackFunc)
        {
            return InitializeMapping(DeviceBindingType.Input, callbackFunc);
        }

        protected DeviceBinding InitializeOutputMapping()
        {
            return InitializeMapping(DeviceBindingType.Output, null);
        }

        private DeviceBinding InitializeMapping(DeviceBindingType deviceBindingType, DeviceBinding.ValueChanged callbackFunc)
        {
            DeviceBinding deviceBinding = new DeviceBinding(callbackFunc);
            switch(deviceBindingType)
            {
                case DeviceBindingType.Input:
                    Inputs.Add(deviceBinding);
                    break;
                case DeviceBindingType.Output:
                    Outputs.Add(deviceBinding);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(deviceBindingType), deviceBindingType, null);
            }
            return deviceBinding;
        }
    }
}
