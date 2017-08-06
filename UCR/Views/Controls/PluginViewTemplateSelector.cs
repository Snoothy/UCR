using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using UCR.Models.Plugins;
using UCR.Models.Plugins.Remapper;

namespace UCR.Views.Controls
{
    class PluginViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PluginTemplate { get; set; }
        public Window Window { get; set; }

        public PluginViewTemplateSelector()
        {
            Window = System.Windows.Application.Current.MainWindow;
        }
        
        //You override this function to select your data template based in the given item
        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item == null) return PluginTemplate;
            PluginTemplate = (DataTemplate)Window.FindResource(item.GetType().Name);
            return PluginTemplate;
        }
    }
}
