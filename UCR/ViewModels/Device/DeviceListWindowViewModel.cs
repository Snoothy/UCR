using UCR.Core;
using UCR.Core.Models.Device;

namespace UCR.ViewModels.Device
{
    public class DeviceListWindowViewModel
    {
        public DeviceListControlViewModel JoystickDeviceList { get; set; }
        public DeviceListControlViewModel KeyboardDeviceList { get; set; }
        public DeviceListControlViewModel MiceDeviceList { get; set; }
        public DeviceListControlViewModel GenericDeviceList { get; set; }

        public DeviceListWindowViewModel(Context context)
        {
            JoystickDeviceList = new DeviceListControlViewModel(context, DeviceType.Joystick);
            KeyboardDeviceList= new DeviceListControlViewModel(context, DeviceType.Keyboard);
            MiceDeviceList = new DeviceListControlViewModel(context, DeviceType.Mouse);
            GenericDeviceList = new DeviceListControlViewModel(context, DeviceType.Generic);
        }

        public DeviceListWindowViewModel()
        {
        }
    }
}