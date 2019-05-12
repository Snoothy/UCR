using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.Dashboard;

namespace HidWizards.UCR.Views.Controls
{
    /// <summary>
    /// Interaction logic for ProfileDeviceListControl.xaml
    /// </summary>
    public partial class ProfileDeviceListControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(ProfileDeviceListControl), new PropertyMetadata(default(string)));

        public ProfileDeviceListControl()
        {
            InitializeComponent();
        }

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        private void AddDevice_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void RemoveDevice_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ProfileDeviceListViewModel;
            viewModel.RemoveDevice(GetSelectedDevice());
        }

        private void DeviceList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as ProfileDeviceListViewModel;
            if (viewModel == null) return;

            viewModel.SelectedDevice = GetSelectedDevice()?.Device;
        }

        private DeviceItem GetSelectedDevice()
        {
            return (DeviceItem) DeviceList.SelectedItem;
        }
    }
}
