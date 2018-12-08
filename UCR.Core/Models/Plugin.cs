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
        /* Persistence */
        public Guid State { get; set; }
        public List<DeviceBinding> Outputs { get; }
        
        /* Runtime */
        internal Profile Profile { get; set; }
        private List<IODefinition> _inputCategories;
        private List<IODefinition> _outputCategories;
        private List<List<PluginProperty>> _guiMatrix;

        #region Properties
        
        [XmlIgnore]
        public List<IODefinition> InputCategories
        {
            get
            {
                if (_inputCategories != null) return _inputCategories;
                _inputCategories = GetIODefinitions(DeviceIoType.Input);
                return _inputCategories;
            }
        }

        [XmlIgnore]
        public List<IODefinition> OutputCategories
        {
            get
            {
                if (_outputCategories != null) return _outputCategories;
                _outputCategories = GetIODefinitions(DeviceIoType.Output);
                return _outputCategories;
            }
        }

        [XmlIgnore]
        public List<List<PluginProperty>> GuiMatrix
        {
            get
            {
                if (_guiMatrix != null) return _guiMatrix;
                _guiMatrix = GetGuiMatrix();
                return _guiMatrix;
            }
        }

        [XmlIgnore]
        public string PluginName => State.Equals(Guid.Empty) ? GetPluginAttribute().Name : $"{GetPluginAttribute().Name} - State: {StateTitle}";

        [XmlIgnore]
        public bool IsDisabled => GetPluginAttribute().Disabled;

        public string StateTitle
        {
            get => Profile.GetStateTitle(State);
        }

        #endregion

        public struct IODefinition
        {
            public string Name;
            public DeviceBindingCategory Category;
        }

        protected Plugin()
        {
            Outputs = new List<DeviceBinding>();
            foreach (var _ in OutputCategories)
            {
                Outputs.Add(new DeviceBinding(null, Profile, DeviceIoType.Output));
            }
        }

        #region Life cycle
        
        public virtual void OnActivate()
        {

        }

        public virtual void OnPropertyChanged()
        {

        }

        /*
         * Called before a plugin OnActivate and OnPropertyChanged to cache run-time values
         */
        public virtual void InitializeCacheValues()
        {

        }

        public virtual void Update(params short[] values)
        {

        }

        public virtual void OnDeactivate()
        {

        }

        #endregion

        #region Plugin methods

        protected void WriteOutput(int number, short value)
        {
            Outputs[number].WriteOutput(value);
        }

        protected void SetState(Guid stateGuid, bool newState)
        {
            Profile.SetRuntimeState(stateGuid, newState);
        }

        protected bool GetState(Guid stateGuid)
        {
            return Profile.GetRuntimeState(stateGuid);
        }

        protected long ReadOutput(int number)
        {
            return Outputs[number].CurrentValue;
        }

        #endregion
        
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

        public void ContextChanged()
        {
            Profile?.Context?.ContextChanged();
        }

        #region Loading

        public void PostLoad(Context context, Profile parentProfile)
        {
            SetProfile(parentProfile);
            ZipDeviceBindingList(Outputs);
            Outputs.ForEach(o => o.DeviceIoType = DeviceIoType.Output);

        }

        private static void ZipDeviceBindingList(IList<DeviceBinding> deviceBindings)
        {
            if (deviceBindings.Count == 0) return;
            var split = deviceBindings.Count / 2;
            for (var i = 0; i < split; i++)
            {
                deviceBindings[i].IsBound = deviceBindings[i + split].IsBound;
                deviceBindings[i].DeviceGuid = deviceBindings[i + split].DeviceGuid;
                deviceBindings[i].KeyType = deviceBindings[i + split].KeyType;
                deviceBindings[i].KeyValue = deviceBindings[i + split].KeyValue;
                deviceBindings[i].KeySubValue = deviceBindings[i + split].KeySubValue;
            }

            for (var i = deviceBindings.Count - 1; i >= split; i--)
            {
                deviceBindings.Remove(deviceBindings[i]);
            }
        }

        #endregion
        
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

        private List<PluginProperty> GetGuiProperties()
        {
            var properties = from p in GetType().GetProperties()
                let attr = p.GetCustomAttributes(typeof(PluginGuiAttribute), true)
                where attr.Length == 1
                select new { Property = p, Attribute = attr.First() as PluginGuiAttribute };


            return properties.Select(prop => new PluginProperty(this, prop.Property, prop.Attribute.Name, prop.Attribute.RowOrder, prop.Attribute.ColumnOrder)).ToList();
        }

        public List<List<PluginProperty>> GetGuiMatrix()
        {
            var result = new List<List<PluginProperty>>();
            var currentRow = new List<PluginProperty>();
            result.Add(currentRow);

            var guiProperties = GetGuiProperties();
            guiProperties.Sort();

            foreach (var pluginProperty in guiProperties)
            {
                if (currentRow.Count != 0 && currentRow[0].RowOrder != pluginProperty.RowOrder)
                {
                    currentRow = new List<PluginProperty>();
                    result.Add(currentRow);
                }

                currentRow.Add(pluginProperty);
            }

            foreach (var pluginPropertiesRow in result)
            {
                pluginPropertiesRow.Sort((x, y) => x.ColumnOrder.CompareTo(y.ColumnOrder));
            }

            return result;
        }

        #endregion
    }
}
