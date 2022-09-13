using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using WPFLab.MVVM;
using WPFLab.Threading;

namespace D3DLab.App.Shell {
    public class ProxyDialog<TWin, TVM> where TWin : Window where TVM : BaseNotify {
        readonly NullVerificationLock<TWin> loker;
        readonly Func<TWin> create;
        TWin win;
        public ProxyDialog(Func<TWin> create) {
            loker = new NullVerificationLock<TWin>();
            this.create = create;
        }
        TWin Create() {
            var win = create();
            win.Owner = Application.Current.MainWindow;
            win.Closed += Win_Closed;
            return win;
        }
        void Win_Closed(object? sender, EventArgs e) {
            loker.Destroy(ref win, Cleanup);
        }

        TWin Cleanup() {
            win.Closed -= Win_Closed;
            return null;
        }

        public void Open() {
            Open(_ => { });
        }
        public void Open(Action<TVM> init) {
            loker.Create(ref win, Create);
            init((TVM)win.DataContext);
            win.Show();
        }
        public void Close() {
            win.Close();
        }

    }
}
