using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Xml.Serialization;
using HidWizards.UCR.Core.Attributes;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models
{
    [InheritedExport(typeof(Plugin))]
    public abstract class Plugin : IComparable<Plugin>
    {
        // Persistence
        public string State { get; set; }
        public List<DeviceBinding> Outputs { get; set; }
        
        // Runtime
        internal Profile Profile { get; set; }
        private List<IODefinition> inputCategories;
        private List<IODefinition> outputCategories;

        [XmlIgnore]
        public List<IODefinition> InputCategories
        {
            get
            {
                if (inputCategories != null) return inputCategories;
                inputCategories = GetIODefinitions(DeviceIoType.Input);
                return inputCategories;
            }
        }

        [XmlIgnore]
        public List<IODefinition> OutputCategories
        {
            get
            {
                if (outputCategories != null) return outputCategories;
                outputCategories = GetIODefinitions(DeviceIoType.Output);
                return outputCategories;
            }
        }

        internal string PluginName => GetPluginAttribute().Name;
        
        public struct IODefinition
        {
            public string Name;
            public DeviceBindingCategory Category;
        }

        #region Life cycle

        protected Plugin()
        {
            Outputs = new List<DeviceBinding>();
            foreach (var _ in OutputCategories)
            {
                Outputs.Add(new DeviceBinding(null, Profile, DeviceIoType.Output));
            }
        }

        public virtual void OnActivate()
        {

        }

        public virtual void Update(List<long> values)
        {

        }

        public virtual void OnDeactivate()
        {

        }

        // TODO 
        protected void WriteOutput(int number, long value)
        {
            Outputs[number].WriteOutput(value);
        }

        #endregion
        
        public Device GetDevice(DeviceBinding deviceBinding)
        {
            return Profile.GetDevice(deviceBinding);
        }

        public List<Device> GetDeviceList(DeviceBinding deviceBinding)
        {
            return Profile.GetDeviceList(deviceBinding);
        }

        
        public void SetProfile(Profile profile)
        {
            Profile = profile;
            Outputs.ForEach(o => o.Profile = profile);
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

        public void PostLoad(Context context, Profile parentProfile)
        {
            SetProfile(parentProfile);
            Outputs.ForEach(o => o.DeviceIoType = DeviceIoType.Output);
        }

        #region Comparison

        public int CompareTo(Plugin other)
        {
            return string.Compare(PluginName, other.PluginName, StringComparison.Ordinal);
        }

        public bool HasSameInputCategories(Plugin other)
        {
            if (InputCategories.Count > other.InputCategories.Count) return false;
            for (var i = 0; i < InputCategories.Count; i++)
            {
                if (InputCategories[i].Category != other.InputCategories[i].Category) return false;
            }

            return true;
        }

        #endregion

        #region Attributes

        private PluginAttribute GetPluginAttribute()
        {
            var pluginAttribute = (PluginAttribute)Attribute.GetCustomAttribute(GetType(), typeof(PluginAttribute));

            return pluginAttribute ?? new PluginAttribute("Invalid plugin");
        }
        
        private List<IODefinition> GetIODefinitions(DeviceIoType deviceIoType)
        {
            var attributes = (PluginIoAttribute[])Attribute.GetCustomAttributes(GetType(), typeof(PluginIoAttribute));
            
            return attributes.Where(a => a.DeviceIoType == deviceIoType).Select(a => new IODefinition()
            {
                Category = a.DeviceBindingCategory,
                Name = a.Name
            }).ToList();
        }

        #endregion
    }
}
