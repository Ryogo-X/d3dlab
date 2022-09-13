using D3DLab.App.Shell.Plugin;
using D3DLab.Plugin;
using D3DLab.Viewer.Presentation.LoadedPanel;

namespace D3DLab.Viewer.Modules.Plugin {
    static class PluginExtentions {
        public static IPluginLoadedObjectDetails ToPluginObject(this LoadedObjectItem obj) 
            => new PluginLoadedObjectDetails(
                   obj.ID,
                   obj.File,
                   obj.Visual.Tags);
        
    }
}
