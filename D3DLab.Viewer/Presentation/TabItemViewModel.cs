using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

using D3DLab.Plugin;

using WPFLab;
using WPFLab.MVVM;

namespace D3DLab.Viewer.Presentation {

    interface ITabPanelContent {
        void Close();
    }

    class TabPanelPluginContent : ITabPanelContent {
        public IPluginViewModel PluginViewModel { get; }

        public TabPanelPluginContent(IPluginViewModel pluginViewModel) {
            PluginViewModel = pluginViewModel;
        }

        public void Close() {
            PluginViewModel.Closed();
        }
    }

    class TabItemViewModel : BaseNotify {
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
}
