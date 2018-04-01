using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.Views.Controls
{
    public class PluginGuiTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var element = container as FrameworkElement;

            if (element == null || !(item is PluginProperty)) return null;
            var pluginProperty = (PluginProperty) item;

            switch (Type.GetTypeCode(pluginProperty.PropertyInfo.PropertyType))
            {
                case TypeCode.Boolean:
                    return element.FindResource("BooleanTemplate") as DataTemplate;
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return element.FindResource("NumberTemplate") as DataTemplate;
                case TypeCode.String:
                    return element.FindResource("StringTemplate") as DataTemplate;
                default:
                    return null;
            }

            
        }
    }
}