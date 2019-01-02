using System.Windows;
using System.Windows.Controls;

namespace HidWizards.UCR.Views.Controls.Plugin
{
    class PluginViewTemplateSelector : DataTemplateSelector
    {
        public DataTemplate PluginTemplate { get; set; }
        public Window Window { get; set; }

        public PluginViewTemplateSelector()
        {
            Window = Application.Current.MainWindow;
        }
        
        //You override this function to select your data template based in the given item
        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item == null) return PluginTemplate;
            try
            {
                PluginTemplate = (DataTemplate)Window.FindResource(item.GetType().Name);
            }
            catch (ResourceReferenceKeyNotFoundException)
            {
                return null;
            }
            
            return PluginTemplate;
        }
    }
}
