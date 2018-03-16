using System;
using System.Collections.Generic;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models
{
    public class Mapping
    {
        public String Title { get; set; }
        public Guid Guid { get; set; }
        public List<DeviceBinding> DeviceBindings { get; set; }
        public List<Plugin> Plugins { get; set; }

        internal Profile Profile { get; set; }

        public Mapping()
        {
            Guid = Guid.NewGuid();
            DeviceBindings = new List<DeviceBinding>();
            Plugins = new List<Plugin>();
        }

        public Mapping(string title) : this()
        {
            Title = title;
        }

        // TODO Add Guid to distinguish devicebindings
        internal void Update(long value)
        {

        }

        internal void PostLoad(Context context, Profile profile = null)
        {
            Profile = profile;

            foreach (var plugin in Plugins)
            {
                plugin.PostLoad(context, profile);
            }
        }

        internal void InitializeMappings(int amount)
        {
            DeviceBindings = new List<DeviceBinding>();
            for (var i = 0; i < amount; i++)
            {
                DeviceBindings.Add(new DeviceBinding(Update, Profile, DeviceIoType.Input));
            }
        }
    }
}
