using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using D3DLab.Plugin;

using WPFLab.MVVM;

namespace D3DLab.App.Shell.Tabs {
    public interface ITabPanelContent {
        void Close();
    }

    public class TabPanelPluginContent : ITabPanelContent {
        public IPluginViewModel PluginViewModel { get; }

        public TabPanelPluginContent(IPluginViewModel pluginViewModel) {
            PluginViewModel = pluginViewModel;
        }

        public void Close() {
            PluginViewModel.Closed();
        }
    }

    public class TabItemViewModel : BaseNotify {
        private bool isSelected;

        public bool IsSelected {
            get {
                return isSelected;
            }
            set {
                Update(ref isSelected, value);
            }
        }
        public string Header { get; private set; }

        public ITabPanelContent Content { get; private set; }

        public TabItemViewModel(string header, ITabPanelContent content) {
            Header = header;
            Content = content;

        }

        public T ContantAs<T>() where T : ITabPanelContent => (T)Content;

        public void Close() {
            Content.Close();
        }
    }


    public static class TabItemEx {
        public static void AddTabWithPluginContent(this ObservableCollection<TabItemViewModel> source,
            string name, IPluginViewModel vm) {
            source.Add(new TabItemViewModel(name, new TabPanelPluginContent(vm)));
        }
    }
}
