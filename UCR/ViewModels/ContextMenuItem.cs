using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HidWizards.UCR.ViewModels
{
    public class ContextMenuItem
    {
        public string Name { get; set; }
        public ObservableCollection<ContextMenuItem> Children { get; set; }
        public ICommand MenuCommand { get; set; }

        public ContextMenuItem(string name, ObservableCollection<ContextMenuItem> pChildren, ICommand pCommand = null)
        {
            Name = name;
            Children = pChildren;
            MenuCommand = pCommand;
        }
    }
}
