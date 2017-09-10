using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using UCR.Models.Devices;
using UCR.Models.Mapping;

namespace UCR.Models.Plugins
{
    public abstract class Plugin
    {
        // Persistence
        public string Title { get; set; }
        public List<DeviceBinding> Inputs { get; set; } // TODO Private
        public List<DeviceBinding> Outputs { get; set; } // TODO Private

        // Runtime
        public delegate void PluginBindingChanged(Plugin plugin);
        internal Profile ParentProfile { get; set; }
        [XmlIgnore]
        public PluginBindingChanged BindingCallback { get; set; }

        // Abstract
        public abstract string PluginName();
        
        protected Plugin()
        {
            Inputs = new List<DeviceBinding>();
            Outputs = new List<DeviceBinding>();
        }

        public bool Activate(UCRContext ctx)
        {
            var success = true;
            success &= SubscribeInputs(ctx);
            return success;
        }

        public Device GetDevice(DeviceBinding deviceBinding)
        {
            return ParentProfile.GetDevice(deviceBinding);
        }

        protected void WriteOutput(DeviceBinding output, long value)
        {
            if (output?.DeviceType == null) return;
            var device = ParentProfile.GetLocalDevice(output);
            device.WriteOutput(ParentProfile.ctx, output, value);
        }

        public virtual List<DeviceBinding> GetInputs()
        {
            return Inputs.Select(input => new DeviceBinding(input)).ToList();
        }

        private bool SubscribeInputs(UCRContext ctx)
        {
            var success = true;
            foreach (var input in GetInputs())
            {
                var device = ctx.ActiveProfile.GetLocalDevice(input);
                if (device != null)
                {
                    success &= device.AddDeviceBinding(input);
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
            var deviceBinding = new DeviceBinding(callbackFunc, this, deviceBindingType);
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

        public List<Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            return ParentProfile.GetDeviceList(deviceBinding);
        }

        public void Rename(string title)
        {
            Title = title;
            ParentProfile.ctx.IsNotSaved = true;
        }

        internal void PostLoad(UCRContext ctx, Profile profile)
        {
            ParentProfile = profile;
            BindingCallback = profile.OnDeviceBindingChange;

            ZipDeviceBindingList(Inputs);
            ZipDeviceBindingList(Outputs);
        }

        private void ZipDeviceBindingList(List<DeviceBinding> deviceBindings)
        {
            if (deviceBindings.Count == 0) return;
            var split = deviceBindings.Count / 2;
            for (var i = 0; i < split; i++)
            {
                deviceBindings[i].IsBound = deviceBindings[i + split].IsBound;
                deviceBindings[i].DeviceType= deviceBindings[i + split].DeviceType;
                deviceBindings[i].DeviceNumber = deviceBindings[i + split].DeviceNumber;
                deviceBindings[i].KeyType = deviceBindings[i + split].KeyType;
                deviceBindings[i].KeyValue = deviceBindings[i + split].KeyValue;
                deviceBindings[i].KeySubValue = deviceBindings[i + split].KeySubValue;
            }

            for (var i = deviceBindings.Count - 1; i >= split ; i--)
            {
                deviceBindings.Remove(deviceBindings[i]);
            }
        }
    }
}
