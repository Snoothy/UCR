using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;

namespace HidWizards.UCR.Core.Models.Plugin
{
    internal class PluginLoader
    {
        private CompositionContainer _Container;

        [ImportMany(typeof(Plugin))]
        public List<Plugin> Plugins { get; set; }

        public PluginLoader(string basePath)
        {
            var catalog = new AggregateCatalog();

            foreach (var path in Directory.EnumerateDirectories(@".\" + basePath, "*", SearchOption.TopDirectoryOnly))
            {
                var folderName = path.Remove(0, path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                if (File.Exists(Path.Combine(path, folderName + ".dll")))
                {
                    catalog.Catalogs.Add(new DirectoryCatalog(path));
                }
            }

            _Container = new CompositionContainer(catalog);
            _Container.ComposeParts(this);
        }
    }
}
