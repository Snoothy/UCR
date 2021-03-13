using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.DeviceViewModels;

namespace HidWizards.UCR.ViewModels.Controls
{
    public class DeviceAddRemoveControlViewModel
    {
        public string TitleLeft { get; set; }
        public string TitleRight { get; set; }
        public ObservableCollection<DeviceViewModel> AvailableDevices { get; set; }
        public ObservableCollection<DeviceViewModel> ShadowDevices { get; set; }

        public DeviceAddRemoveControlViewModel()
        {
        }

        public DeviceAddRemoveControlViewModel(string titleLeft, string titleRight, List<DeviceViewModel> devices)
        {
            TitleLeft = titleLeft;
            TitleRight = titleRight;
            var deviceViewModels = new ObservableCollection<DeviceViewModel>(devices);

            AvailableDevices = new ObservableCollection<DeviceViewModel>(deviceViewModels.Where(d => !d.Checked).ToList());
            ShadowDevices = new ObservableCollection<DeviceViewModel>(deviceViewModels.Where(d => d.Checked).ToList());

            AvailableDevices.CollectionChanged += Devices_CollectionChanged;
            ShadowDevices.CollectionChanged += Devices_CollectionChanged;

            SetFirstElement(AvailableDevices);
            SetFirstElement(ShadowDevices);
        }

        private void Devices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var collection = sender as ObservableCollection<DeviceViewModel>;
            SetFirstElement(collection);
        }

        public void AddShadowDevice(DeviceViewModel device)
        {
            if (AvailableDevices.Remove(device))
            {
                device.Checked = true;
                ShadowDevices.Add(device);
            }
        }

        public void RemoveShadowDevice(DeviceViewModel device)
        {
            if (ShadowDevices.Remove(device))
            {
                device.Checked = false;
                AvailableDevices.Add(device);
            }
        }

        private void SetFirstElement(ObservableCollection<DeviceViewModel> deviceList)
        {
            if (deviceList == null || deviceList.Count == 0) return;

            foreach (var deviceViewModel in deviceList)
            {
                deviceViewModel.FirstElement = false;
            }
            
            deviceList[0].FirstElement = true;
        }
    }
}
