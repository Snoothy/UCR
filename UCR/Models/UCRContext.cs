using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UCR.Models.Devices;

namespace UCR.Models
{
    public class UCRContext
    {
        public bool IsNotSaved { get; set; }
        public List<Profile> Profiles { get; set; }

        // Device lists
        public List<Keyboard> Keyboards { get; set; }
        public List<Mouse> Mice { get; set; }
        public List<Joystick> Joysticks { get; set; }

        public UCRContext()
        {
            IsNotSaved = false;
            Init();
        }

        public void Init()
        {
            InitMock();
        }

        public void ActivateProfile(Profile profile)
        {
            bool success = true;
            success &= GetGlobalProfile().Activate(this);
            success &= profile.Activate(this);
            if (success) SubscribeDeviceLists();
        }



        private void SubscribeDeviceLists()
        {
            // TODO Subscribe the bindings to actual input
        }

        private Profile GetGlobalProfile()
        {
            // TODO Find it properly
            return Profiles.Find(p => p.Title.Equals("Global"));
        }

        private void InitMock()
        {
            Profiles = new List<Profile>
            {
                new Profile(null)
                {
                    Title = "Default"
                },
                new Profile(null)
                {
                    Title = "Some profile"
                }
            };
        }
    }
}
