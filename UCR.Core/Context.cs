using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using HidWizards.IOWrapper.Core;
using HidWizards.UCR.Core.Annotations;
using HidWizards.UCR.Core.Managers;
using HidWizards.UCR.Core.Models;
using Mono.Options;
using NLog;

namespace HidWizards.UCR.Core
{
    public sealed class Context : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string ContextName = "context.xml";
        private const string PluginPath = "Plugins";

        /* Persistence */
        public List<Profile> Profiles { get; set; }

        /* Runtime */
        [XmlIgnore] public Profile ActiveProfile { get; set; }
        [XmlIgnore] public ProfilesManager ProfilesManager { get; set; }
        [XmlIgnore] public DevicesManager DevicesManager { get; set; }
        [XmlIgnore] public SubscriptionsManager SubscriptionsManager { get; set; }
        [XmlIgnore] public PluginsManager PluginManager { get; set; }
        [XmlIgnore] public BindingManager BindingManager { get; set; }

        public delegate void ActiveProfileChanged(Profile profile);
        public event ActiveProfileChanged ActiveProfileChangedEvent;
        
        internal bool IsNotSaved { get; private set; }
        internal IOController IOController { get; set; }
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

            try
            {
                IOController = new IOController();
            }
            catch (DirectoryNotFoundException e)
            {
                Logger.Error("IOWrapper provider directory not found", e);
            }
            
            ProfilesManager = new ProfilesManager(this, Profiles);
            DevicesManager = new DevicesManager(this);
            SubscriptionsManager = new SubscriptionsManager(this);
            PluginManager = new PluginsManager(PluginPath);
            BindingManager = new BindingManager(this);
        }

        private void SetCommandLineOptions()
        {
            options = new OptionSet {
                { "p|profile=", "The profile to search for", FindAndLoadProfile }
            };
        }

        private void FindAndLoadProfile(string profileString)
        {
            Logger.Debug($"Searching for profile to load: {{{profileString}}}");
            var search = profileString.Split(',').ToList();
            var profile = ProfilesManager.FindProfile(search);
            if (profile != null) SubscriptionsManager.ActivateProfile(profile);
        }

        public void ParseCommandLineArguments(IEnumerable<string> args)
        {
            options.Parse(args);
        }

        public List<Plugin> GetPlugins()
        {
            return PluginManager.Plugins.Where(p => !p.IsDisabled).ToList();
        }

        public void ContextChanged()
        {
            Logger.Trace("Context changed");
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
                Logger.Error("Failed to load context.xml", e);
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
            var plugins = new PluginsManager(PluginPath);
            var pluginTypes = plugins.Plugins.Select(p => p.GetType()).ToList();
            if (additionalPluginTypes != null) pluginTypes.AddRange(additionalPluginTypes);
            return new XmlSerializer(type, pluginTypes.ToArray());
        }

        #endregion

        private string GetVersion()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            return fileVersionInfo.ProductVersion;
        }

        public void Dispose()
        {
            SubscriptionsManager.Dispose();
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

        public void OnActiveProfileChangedEvent(Profile profile)
        {
            ActiveProfileChangedEvent?.Invoke(profile);
        }
    }
}