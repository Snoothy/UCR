using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UCR.Models.Devices;
using UCR.Models.Plugins;
using UCR.Models.Mapping;
using UCR.ViewModels;
using Binding = UCR.Models.Mapping.Binding;

namespace UCR.Models
{
    public class Profile
    {

        public String Title { get; set; }
        public Profile Parent { get; set; }
        public long Id { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Plugin> Plugins { get; set; }

        public DeviceGroup<Device> Keyboards { get; set; }
        public DeviceGroup<Device> Mice { get; set; }
        public DeviceGroup<Device> Joysticks { get; set; }

        public bool InheritFromParent { get; set; }

        public Profile(Profile parent)
        {
            Parent = parent;
            Plugins = new List<Plugin>();
            InheritFromParent = true;
        }

        public bool Activate(UCRContext ctx)
        {
            bool success = true;
            foreach (var plugin in Plugins)
            {
                success &= plugin.Activate(ctx);
            }

            if (Parent != null && InheritFromParent)
            {
                Parent.Activate(ctx);
            }

            return success;
        }

        public void AddNewChildProfile(String title)
        {
            if (ChildProfiles == null) ChildProfiles = new List<Profile>();
            ChildProfiles.Add(CreateProfile(title, this));
        }

        public void Delete(UCRContext ctx)
        {
            if (Parent == null)
            {
                ctx.Profiles.Remove(this);
            }
            else
            {
                Parent.ChildProfiles.Remove(this);
            }
        }

        public static Profile CreateProfile(String title, Profile parent)
        {
            Profile profile = new Profile(parent)
            {
                Title = title
            };

            return profile;
        }

        public Device GetDevice(Binding binding)
        {
            List<Device> deviceList;
            switch (binding.DeviceType)
            {
                case DeviceType.Keyboard:
                    deviceList = Keyboards.Devices;
                    break;
                case DeviceType.Mouse:
                    deviceList = Mice.Devices;
                    break;
                case DeviceType.Joystick:
                    deviceList = Joysticks.Devices;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (binding.DeviceNumber <= deviceList.Count)
            {
                return deviceList[binding.DeviceNumber];
            }
            if (InheritFromParent && Parent != null)
            {
                return Parent.GetDevice(binding);
            }
            return null;
        }

    }
}
