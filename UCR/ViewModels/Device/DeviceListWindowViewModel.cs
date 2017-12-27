using UCR.Core;
using UCR.Core.Models.Device;

namespace UCR.ViewModels.Device
{
    public class DeviceListWindowViewModel
    {
        public DeviceListControlViewModel InputDeviceList { get; set; }
        public DeviceListControlViewModel OutputDeviceList { get; set; }

        public DeviceListWindowViewModel(Context context)
        {
            InputDeviceList = new DeviceListControlViewModel(context, DeviceIoType.Input);
            OutputDeviceList = new DeviceListControlViewModel(context, DeviceIoType.Output);

        }

        public DeviceListWindowViewModel()
        {
        }
    }
}