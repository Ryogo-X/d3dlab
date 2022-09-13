using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;
using D3DLab.Plugin;

using WPFLab.MVVM;

namespace D3DLab.App.Shell.Plugin {
    public interface IPluginHandler {
        public void Handle(LoadedPlugin loaded);
    }

    public class PluginItemViewModel {

        public string Name => Plugin.Plugin.Name;
        public string Description => Plugin.Plugin.Description;

        public readonly LoadedPlugin Plugin;

        public PluginItemViewModel(LoadedPlugin p) {
            this.Plugin = p;
        }
    }

    public class PluginsViewModel : BaseNotify {

        public ICollectionView Plugins { get; set; }

        readonly ObservableCollection<PluginItemViewModel> plugins;
        readonly IPluginHandler handler;

        public PluginsViewModel(IPluginHandler handler) {
            plugins = new ObservableCollection<PluginItemViewModel>();
            Plugins = CollectionViewSource.GetDefaultView(plugins);
            Plugins.CurrentChanged += OnCurrentItemChanged;
            this.handler = handler;
        }

        private void OnCurrentItemChanged(object? sender, EventArgs e) {
            if(Plugins.CurrentItem is PluginItemViewModel item) {
                handler.Handle(item.Plugin);
            }
        }

        public void SetPluginProxy(PluginProxy proxy) {
            foreach(var p in proxy.Plugins) {
                plugins.Add(new PluginItemViewModel(p));
            }
        }
    }
}
