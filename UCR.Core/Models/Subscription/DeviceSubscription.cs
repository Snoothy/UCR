using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCR.Core.Models.Subscription
{
    public class DeviceSubscription
    {
        public Device.Device Device { get; }
        public Profile.Profile Profile { get; }

        public DeviceSubscription(Device.Device device, Profile.Profile profile)
        {
            Device = device;
            Profile = profile;
        }
    }
}
