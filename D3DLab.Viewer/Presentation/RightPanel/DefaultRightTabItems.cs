using System.Collections.ObjectModel;

using D3DLab.App.Shell.Tabs;

namespace D3DLab.Viewer.Presentation.RightPanel {
    static class DefaultRightTabItems {
        public static void AddGeneralTab(this ObservableCollection<TabItemViewModel> source) {

            source.Add(new TabItemViewModel("General", new GeneralTabContentViewModel()));
        }
    }
}
