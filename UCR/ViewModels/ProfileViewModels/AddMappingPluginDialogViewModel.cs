using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using HidWizards.UCR.Core.Annotations;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class AddMappingPluginDialogViewModel : INotifyPropertyChanged
    {
        public string Title => $"Add plugin to mapping: {MappingViewModel?.MappingTitle}";
        public AddMappingPluginDialogViewModel ViewModel { get; set; }
        private MappingViewModel MappingViewModel { get; }

        public List<SimplePluginViewModel> Plugins { get; set; }
        public SimplePluginViewModel SelectedPlugin { get; set; }
        public bool InputValid => SelectedPlugin != null;

        public AddMappingPluginDialogViewModel()
        {
        }

        public AddMappingPluginDialogViewModel(MappingViewModel mappingViewModel)
        {
            ViewModel = this;
            Plugins = new List<SimplePluginViewModel>();
            MappingViewModel = mappingViewModel;

            mappingViewModel.Mapping.GetPluginList().ForEach(p => Plugins.Add(new SimplePluginViewModel(p)));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SelectionChanged()
        {
            OnPropertyChanged(nameof(InputValid));
        }
    }
}
