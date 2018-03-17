using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models
{
    public abstract class Plugin : IComparable<Plugin>
    {
        // Persistence
        public string Title { get; set; }
        public string State { get; set; }
        public DeviceBinding Output { get; set; }
        
        // Runtime
        internal Profile Profile { get; set; }

        // Abstract
        [XmlIgnore]
        public abstract string PluginName { get; }
        [XmlIgnore]
        public abstract DeviceBindingCategory OutputCategory { get; }
        protected abstract List<PluginInput> InputCategories { get; }

        public struct PluginInput
        {
            public string Name;
            public DeviceBindingCategory Category;
        }

        protected Plugin()
        {
            Output = new DeviceBinding(null, Profile, DeviceIoType.Output);
        }

        public List<PluginInput> GetInputCategories()
        {
            return InputCategories;
        }

        public bool Remove()
        {
            Profile.Context.ContextChanged();
            return true;
        }

        public virtual void OnActivate()
        {
            
        }

        public virtual void OnDeactivate()
        {

        }

        public virtual long Update(List<long> values)
        {
            return 0L;
        }

        public Device GetDevice(DeviceBinding deviceBinding)
        {
            return Profile.GetDevice(deviceBinding);
        }

        public List<Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            return Profile.GetDeviceList(deviceBinding);
        }

        // TODO 
        internal void WriteOutput(long value)
        {
            Output.WriteOutput(value);
        }
        
        public void Rename(string title)
        {
            Title = title;
            Profile.Context.ContextChanged();
        }

        public void PostLoad(Context context, Profile parentProfile)
        {
            SetProfile(parentProfile);
        }

        public void SetProfile(Profile profile)
        {
            Profile = profile;
            Output.Profile = profile;
        }

        public Plugin Duplicate()
        {
            var newPlugin = Context.DeepXmlClone(this);
            newPlugin.PostLoad(Profile.Context, Profile);
            return newPlugin;
        }

        protected void ContextChanged()
        {
            Profile?.Context?.ContextChanged();
        }

        public int CompareTo(Plugin other)
        {
            return string.Compare(PluginName, other.PluginName, StringComparison.Ordinal);
        }
    }
}
