using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3DLab.App.Shell.Plugin;
using D3DLab.App.Shell;
using WPFLab;

namespace D3DLab.Sandbox.App.Presentation {
    internal class DialogManager {
        public DialogManager(IDependencyResolverService service) {
            Plugins = new ProxyDialog<PluginsWindow, PluginsViewModel>(
                () => service.ResolveView<PluginsWindow, PluginsViewModel>());
        }

        public ProxyDialog<PluginsWindow, PluginsViewModel> Plugins { get; }
    }
}
