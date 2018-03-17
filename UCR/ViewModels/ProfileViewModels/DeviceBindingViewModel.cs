using HidWizards.UCR.Core.Models.Binding;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class DeviceBindingViewModel
    {
        public string DeviceBindingName { get; set; }
        public DeviceBindingCategory DeviceBindingCategory { get; set; }
        public DeviceBinding DeviceBinding { get; set; }
    }
}
