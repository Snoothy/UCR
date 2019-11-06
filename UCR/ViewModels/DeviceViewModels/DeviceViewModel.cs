using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.DeviceViewModels
{
    public class DeviceViewModel : INotifyPropertyChanged
    {

        public string Title { get; set; }
        public string ProviderName { get; set; }
        public bool Checked { get; set; }

        public Visibility SeparatorVisibility => FirstElement ? Visibility.Collapsed : Visibility.Visible;
        private bool _firstElement;

        public bool FirstElement
        {
            get => _firstElement;
            set
            {
                _firstElement = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SeparatorVisibility));
            }
        }

        public Device Device { get; }

        public DeviceViewModel()
        {
        }

        public DeviceViewModel(Device device)
        {
            Device = device;
            Title = device.Title;
            ProviderName = device.ProviderName;
        }

        public DeviceViewModel(Device device, bool selected) : this(device)
        {
            Checked = selected;
        }

        public void ToggleSelection()
        {
            Checked = !Checked;
            OnPropertyChanged(nameof(Checked));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
