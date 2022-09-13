using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using WPFLab;

namespace D3DLab.App.Shell.TopPanel {
    public interface ITopPanelViewModel {
        ICommand OpenFilesCommand { get; }
        ICommand CameraFocusToAllCommand { get; }
        //ICommand MoveToCenterWorld { get; }
        //ICommand ShowAxis { get; }
        ICommand SaveAllCommand { get; }
        ICommand OpenPluginsWindowCommand { get; }


        BaseWPFCommand<bool> ShowWorldCoordinateSystemCommand { get; }
        BaseWPFCommand<bool> ManipulatorToolEnabledCommand { get; }
      
       // BaseWPFCommand<bool> InformationModeEnabledCommand { get; }
     
        GraphicsInfo GraphicsInfo { get; }
    }
}
