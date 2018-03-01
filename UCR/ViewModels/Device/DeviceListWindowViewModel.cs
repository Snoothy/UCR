using HidWizards.UCR.Core;
using HidWizards.UCR.Core.Models.Device;

namespace HidWizards.UCR.ViewModels.Device
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