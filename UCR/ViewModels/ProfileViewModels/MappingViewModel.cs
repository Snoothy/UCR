using HidWizards.UCR.Core.Models;

namespace HidWizards.UCR.ViewModels.ProfileViewModels
{
    public class MappingViewModel
    {
        public ProfileViewModel ProfileViewModel { get; }
        public Mapping Mapping { get; set; }

        public MappingViewModel(ProfileViewModel profileViewModel, Mapping mapping)
        {
            ProfileViewModel = profileViewModel;
            Mapping = mapping;
        }

        public void Remove()
        {
            ProfileViewModel.RemoveMapping(this);
        }
    }
}
