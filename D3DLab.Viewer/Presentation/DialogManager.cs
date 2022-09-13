using D3DLab.App.Shell;
using D3DLab.App.Shell.Plugin;
using D3DLab.Viewer.Presentation.FileDetails;
using D3DLab.Viewer.Presentation.OpenFiles;
using D3DLab.Viewer.Presentation.TopPanel.SaveAll;

using WPFLab;

namespace D3DLab.Viewer.Presentation {
    class DialogManager {
        public DialogManager(IDependencyResolverService service) {
            ObjDetails = new ProxyDialog<ObjDetailsWindow, ObjDetailsViewModel>(
                ()=> service.ResolveView<ObjDetailsWindow, ObjDetailsViewModel>());
            OpenFiles = new ProxyDialog<OpenFilesWindow, OpenFilesViewModel>(
                () => service.ResolveView<OpenFilesWindow, OpenFilesViewModel>());
            SaveAll = new ProxyDialog<SaveAllWindow, SaveAllViewModel>(
               () => service.ResolveView<SaveAllWindow, SaveAllViewModel>());
            Plugins = new ProxyDialog<PluginsWindow, PluginsViewModel>(
                () => service.ResolveView<PluginsWindow, PluginsViewModel>());
        }

        public ProxyDialog<ObjDetailsWindow, ObjDetailsViewModel> ObjDetails { get; }
        public ProxyDialog<OpenFilesWindow, OpenFilesViewModel>  OpenFiles { get; }
        public ProxyDialog<SaveAllWindow, SaveAllViewModel>  SaveAll { get; }

        public ProxyDialog<PluginsWindow, PluginsViewModel> Plugins { get; }
    }
}
