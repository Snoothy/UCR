using System;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core.Models.Binding;
using HidWizards.UCR.ViewModels.ProfileViewModels;

namespace HidWizards.UCR.Views.Controls
{
    class PreviewControlTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (element == null || !(item is DeviceBindingViewModel)) return null;
            var deviceBindingViewModel = (DeviceBindingViewModel) item;

            switch (deviceBindingViewModel.DeviceBindingCategory)
            {
                case DeviceBindingCategory.Event:
                    return element.FindResource("EventPreview") as DataTemplate;
                case DeviceBindingCategory.Momentary:
                    return element.FindResource("MomentaryPreview") as DataTemplate;
                case DeviceBindingCategory.Range:
                    return element.FindResource("RangePreview") as DataTemplate;
                case DeviceBindingCategory.Delta:
                    return element.FindResource("DeltaPreview") as DataTemplate;
                default:
                    return null;
            }
        }
    }
}
