using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Windows;

namespace HidWizards.UCR.Utilities
{
    public class ResourceLoader
    {
        private CompositionContainer _Container;

        [ImportMany(typeof(ResourceDictionary))]
        private IEnumerable<ResourceDictionary> _resourceDictionaries { get; set; }

        public void Load()
        {
            var catalog = new AggregateCatalog();

            foreach (var path in Directory.EnumerateDirectories(@".\Plugins", "*", SearchOption.TopDirectoryOnly))
            {
                var folderName = path.Remove(0, path.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                if (File.Exists(Path.Combine(path, folderName + ".dll")))
                {
                    catalog.Catalogs.Add(new DirectoryCatalog(path, folderName + ".dll"));
                }
            }

            _Container = new CompositionContainer(catalog);
            _Container.ComposeParts(this);

            foreach (var resourceDictionary in _resourceDictionaries)
            {
                Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
        }
    }
}