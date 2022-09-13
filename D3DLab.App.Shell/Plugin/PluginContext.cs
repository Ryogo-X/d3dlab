using System.IO;
using System.Text;
using System.Windows;

using D3DLab.Plugin;

namespace D3DLab.App.Shell.Plugin {
    public class PluginContext : IPluginContext {
        public IPluginScene Scene { get; }
        public DirectoryInfo PluginDirectory { get; }
        public PluginObservableCollection Collection { get; }

        public PluginContext(IPluginScene scene, DirectoryInfo directory, PluginObservableCollection loadedObjects) {
            Scene = scene;
            PluginDirectory = directory;
            Collection = loadedObjects;
        }

        public void AddResource(ResourceDictionary resource) {
            Application.Current.Resources.MergedDictionaries.Add(resource); 
        }
    }
}
