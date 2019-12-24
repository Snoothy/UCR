using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace HidWizards.UCR.Core.Models.Binding
{
    public class DeviceBindingInfo
    {

        [XmlAttribute]
        public int KeyType { get; set; }
        [XmlAttribute]
        public int KeyValue { get; set; }
        [XmlAttribute]
        public int KeySubValue { get; set; }
        [XmlAttribute]
        public DeviceBindingCategory DeviceBindingCategory { get; set; }
        [XmlAttribute]
        public bool Blockable { get; set; }

    }
}
