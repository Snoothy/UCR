using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using IOWrapper;
using Mono.Options;
using UCR.Core.Managers;
using UCR.Core.Models.Device;
using UCR.Core.Models.Plugin;
using UCR.Core.Models.Profile;

namespace UCR.Core
{
    public class Context : IDisposable
    {
        private const string ContextName = "context.xml";
        private const string PluginPath = "Plugins";

        // Persistence
        public List<Profile> Profiles { get; set; }
        public List<DeviceGroup> KeyboardGroups { get; set; }
        public List<DeviceGroup> MiceGroups { get; set; }
        public List<DeviceGroup> JoystickGroups { get; set; }
        public List<DeviceGroup> GenericDeviceGroups { get; set; }

        // Runtime
        [XmlIgnore]
        public Profile ActiveProfile { get; set; }
        [XmlIgnore]
        public ProfilesManager ProfilesManager { get; set; }
        [XmlIgnore]
        public DevicesManager DevicesManager { get; set; }
        [XmlIgnore]
        public DeviceGroupsManager DeviceGroupsManager { get; set; }

        internal bool IsNotSaved { get; private set; }
        internal IOController IOController { get; set; }
        internal readonly List<Action> ActiveProfileCallbacks = new List<Action>();
        private PluginLoader _pluginLoader;
        private OptionSet options;

        public Context()
        {
            Init();
            SetCommandLineOptions();
        }

        private void Init()
        {
            IsNotSaved = false;
            Profiles = new List<Profile>();
            KeyboardGroups = new List<DeviceGroup>();
            MiceGroups = new List<DeviceGroup>();
            JoystickGroups = new List<DeviceGroup>();
            GenericDeviceGroups = new List<DeviceGroup>();

            IOController = new IOController();
            ProfilesManager = new ProfilesManager(this, Profiles);
            DevicesManager = new DevicesManager(this);
            DeviceGroupsManager = new DeviceGroupsManager(this, JoystickGroups, KeyboardGroups, MiceGroups, GenericDeviceGroups);
            _pluginLoader = new PluginLoader(PluginPath);
        }

        private void SetCommandLineOptions()
        {
            options = new OptionSet {
                { "p|profile=", "The profile to search for", LoadProfile }
            };
        }

        private void LoadProfile(string profileString)
        {
            var search = profileString.Split(',').ToList();
            var profile = ProfilesManager.FindProfile(search);
            if (profile != null) ProfilesManager.ActivateProfile(profile);
        }

        public void ParseCommandLineArguments(IEnumerable<string> args)
        {
            options.Parse(args);
        }

        public void SetActiveProfileCallback(Action profileActivated)
        {
            ActiveProfileCallbacks.Add(profileActivated);
        }

        public List<Plugin> GetPlugins()
        {
            return _pluginLoader.Plugins;
        }

        public void ContextChanged()
        {
            IsNotSaved = true;
        }

        #region Persistence
        
        public bool SaveContext(List<Type> pluginTypes = null)
        {
            var serializer = GetXmlSerializer(pluginTypes);
            using (var streamWriter = new StreamWriter(ContextName))
            {
                serializer.Serialize(streamWriter, this);
            }
            IsNotSaved = false;
            return true;
        }

        public static Context Load(List<Type> pluginTypes = null)
        {
            Context context;
            var serializer = GetXmlSerializer(pluginTypes);
            try
            {
                using (var fileStream = new FileStream(ContextName, FileMode.Open))
                {
                    context = (Context) serializer.Deserialize(fileStream);
                    context.PostLoad();
                }
            }
            catch (IOException e)
            {
                Console.Write(e.ToString());
                // TODO log exception
                context = new Context();
            }
            return context;
        }

        private void PostLoad()
        {
            foreach (var profile in Profiles)
            {
                profile.PostLoad(this);
            }
        }

        private static XmlSerializer GetXmlSerializer(List<Type> additionalPluginTypes)
        {
            return GetXmlSerializer(additionalPluginTypes, typeof(Context));
        }

        private static XmlSerializer GetXmlSerializer(List<Type> additionalPluginTypes, Type type)
        {
            var plugins = new PluginLoader(PluginPath);
            var pluginTypes = plugins.Plugins.Select(p => p.GetType()).ToList();
            if (additionalPluginTypes != null) pluginTypes.AddRange(additionalPluginTypes);
            return new XmlSerializer(type, pluginTypes.ToArray());
        }

        #endregion

        public void Dispose()
        {
            IOController?.Dispose();
        }

        public static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        public static T DeepXmlClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = GetXmlSerializer(null, typeof(T));
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}