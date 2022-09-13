using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WPFLab.MVVM;

namespace D3DLab.App.Shell.TopPanel {
    public class GraphicsInfo : BaseNotify {
        private double fps;
        private double milliseconds;
        private string? adapter;

        public string? Adapter { get => adapter; set => Update(ref adapter, value); }
        public double Fps { get => fps; set => Update(ref fps, value); }
        public double Milliseconds { get => milliseconds; set => Update(ref milliseconds, value); }
    }
}
