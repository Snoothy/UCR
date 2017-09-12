using System.Windows;
using System.Windows.Controls;

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
