using System.Windows;
using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class FilterViewModel
    {

        public string Name => Filter.Name;
        public bool Negative => Filter.Negative;
        public Filter Filter { get; }

        private readonly PluginViewModel _pluginViewModel;

        public FilterViewModel(PluginViewModel pluginViewModel, Filter filter)
        {
            _pluginViewModel = pluginViewModel;
            Filter = filter;
        }

        public void RemoveFilter()
        {
            _pluginViewModel.RemoveFilter(this);
        }

    }
}