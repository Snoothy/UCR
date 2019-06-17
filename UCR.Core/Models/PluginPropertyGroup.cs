using System.Collections.Generic;

namespace HidWizards.UCR.Core.Models
{
    public class PluginPropertyGroup
    {
        public string Title { get; set; }
        public string GroupName { get; set; }
        public List<PluginProperty> PluginProperties { get; set; }
        public GroupTypes GroupType { get; set; }

        public enum GroupTypes
        {
            Settings,
            Output
        }
    }
}