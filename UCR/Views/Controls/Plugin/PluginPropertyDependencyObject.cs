using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HidWizards.UCR.Core.Models;
using HidWizards.UCR.ViewModels.ProfileViewModels;

namespace HidWizards.UCR.Views.Controls.Plugin
{
    public class PluginPropertyDependencyObject : DependencyObject
    {
        public static readonly DependencyProperty PluginPropertyProperty = DependencyProperty.Register("PluginProperty", typeof(PluginProperty), typeof(PluginPropertyDependencyObject), new PropertyMetadata(default(PluginProperty)));

        public PluginProperty PluginProperty
        {
            get { return (PluginProperty) GetValue(PluginPropertyProperty); }
            set { SetValue(PluginPropertyProperty, value); }
        }
    }
}
