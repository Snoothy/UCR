using UCR.Models;
using UCR.Models.Devices;
using UCR.Views.Device;

namespace UCR.ViewModels.Device
{
    public class DeviceListWindowViewModel
    {
        private UCRContext ctx;
        public DeviceListControlViewModel JoystickDeviceList { get; set; }
        public DeviceListControlViewModel KeyboardDeviceList { get; set; }
        public DeviceListControlViewModel MiceDeviceList { get; set; }
        public DeviceListControlViewModel GenericDeviceList { get; set; }

        public DeviceListWindowViewModel(UCRContext ctx)
        {
            this.ctx = ctx;
            JoystickDeviceList = new DeviceListControlViewModel(ctx, DeviceType.Joystick);
            KeyboardDeviceList= new DeviceListControlViewModel(ctx, DeviceType.Keyboard);
            MiceDeviceList = new DeviceListControlViewModel(ctx, DeviceType.Mouse);
            GenericDeviceList = new DeviceListControlViewModel(ctx, DeviceType.Generic);
        }

        public DeviceListWindowViewModel()
        {
        }
    }
}