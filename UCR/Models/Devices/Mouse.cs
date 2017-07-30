using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Mapping;

namespace UCR.Models.Devices
{
    public sealed class Mouse : Device
    {
        public Mouse() : base(DeviceType.Mouse)
        {
        }

        Mouse(Mouse mouse) : base(mouse)
        {
            // TODO copy vars
        }

        public override bool Subscribe(Binding binding)
        {
            throw new NotImplementedException();
        }

        public override void ClearSubscribers()
        {
            throw new NotImplementedException();
        }

        public override void Activate(UCRContext ctx)
        {
            throw new NotImplementedException();
        }
    }
}
