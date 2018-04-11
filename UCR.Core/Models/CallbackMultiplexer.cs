using System;
using System.Collections.Generic;
using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.Core.Models
{   
    public class CallbackMultiplexer
    {
        private DeviceBinding.ValueChanged _mappingUpdate;
        private readonly int _index;
        private readonly List<long> _cache;

        public CallbackMultiplexer(List<long> cache, int index, DeviceBinding.ValueChanged mappingUpdate)
        {
            _mappingUpdate = mappingUpdate;
            _index = index;
            _cache = cache;
        }

        public void Update(long value)
        {
            _cache[_index] = value;
            _mappingUpdate(value);
        }

        ~CallbackMultiplexer()
        {
            _mappingUpdate = null;
        }
    }
}
