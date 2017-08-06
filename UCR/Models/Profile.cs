using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using UCR.Models.Devices;
using UCR.Models.Plugins;
using UCR.Models.Mapping;
using UCR.Utilities;
using UCR.ViewModels;

namespace UCR.Models
{
    public class Profile
    {
        public static String GlobalProfileTitle = "Global";

        // Persistence
        public String Title { get; set; }
        public Profile Parent { get; set; }
        public long Guid { get; set; }
        public List<Profile> ChildProfiles { get; set; }
        public List<Plugin> Plugins { get; set; }
        public String KeyboardList { get; set; }
        public String MiceList { get; set; }
        public String JoystickList { get; set; }

        // Runtime
        public DeviceGroup<Keyboard> Keyboards { get; set; }
        public DeviceGroup<Mouse> Mice { get; set; }
        public DeviceGroup<Joystick> Joysticks { get; set; }

        public bool InheritFromParent { get; set; }

        public Profile()
        {
            Plugins = new List<Plugin>();
            InheritFromParent = true;
        }

        public Profile(Profile parent = null) : base()
        {
            Parent = parent;
        }

        public bool Activate(UCRContext ctx)
        {
            bool success = true;
            InitializeDeviceGroups(ctx);
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

        public void InitializeDeviceGroups(UCRContext ctx)
        {
            Joysticks = new DeviceGroup<Joystick>()
            {
                GUID = JoystickList,
                Devices = Device.CopyDeviceList<Joystick>(DeviceGroup<Joystick>.FindDeviceGroup(ctx.JoystickGroups, JoystickList)?.Devices)
            };

            Keyboards = new DeviceGroup<Keyboard>()
            {
                GUID = KeyboardList,
                Devices = Device.CopyDeviceList<Keyboard>(DeviceGroup<Keyboard>.FindDeviceGroup(ctx.KeyboardGroups, KeyboardList)?.Devices)
            };

            Mice = new DeviceGroup<Mouse>()
            {
                GUID = MiceList,
                Devices = Device.CopyDeviceList<Mouse>(DeviceGroup<Mouse>.FindDeviceGroup(ctx.MiceGroups, MiceList)?.Devices)
            };

            // TODO Generic

        }

        public void AddNewChildProfile(string title)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
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
            ctx.IsNotSaved = true;
        }

        public bool Rename(string title)
        {
            if (IsGlobalProfileTitle(title)) return false;
            Title = title;
            return true;
        }
        public static Profile CreateProfile(string title, Profile parent = null)
        {
            if (IsGlobalProfileTitle(title)) title += " not allowed";
            Profile profile = new Profile(parent)
            {
                Title = title
            };

            return profile;
        }

        public Device GetDevice(DeviceBinding deviceBinding)
        {
            dynamic deviceList;
            switch (deviceBinding.DeviceType)
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
            if (deviceBinding.DeviceNumber < deviceList.Count)
            {
                return deviceList[deviceBinding.DeviceNumber];
            }
            if (InheritFromParent && Parent != null)
            {
                // TODO Parent devices should be fetched to the active profile instead of using parents cache
                return Parent.GetDevice(deviceBinding);
            }
            return null;
        }

        public void AddPlugin(Plugin plugin)
        {
            Plugins.Add(plugin);
        }

        public static bool IsGlobalProfileTitle(string title)
        {
            return string.Compare(title, GlobalProfileTitle, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

    }
}
