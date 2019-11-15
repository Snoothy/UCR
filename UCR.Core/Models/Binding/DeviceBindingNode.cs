using System.Collections.Generic;
using System.Xml.Serialization;

namespace HidWizards.UCR.Core.Models.Binding
{
    public class DeviceBindingNode
    {
        [XmlAttribute]
        public string Title { get; set; }
        public bool IsBinding => ChildrenNodes == null || ChildrenNodes.Count == 0;
        public List<DeviceBindingNode> ChildrenNodes { get; set; }
        public DeviceBindingInfo DeviceBindingInfo { get; set; }
    }
}
