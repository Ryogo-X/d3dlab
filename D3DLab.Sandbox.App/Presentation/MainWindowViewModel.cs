using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using D3DLab.App.Shell.D3D;
using D3DLab.App.Shell.Plugin;
using D3DLab.App.Shell.TopPanel;
using D3DLab.ECS.Context;
using D3DLab.ECS;
using D3DLab.Plugin;
using D3DLab.Toolkit.Host;

using WPFLab;
using WPFLab.MVVM;
using D3DLab.Toolkit;
using System.Numerics;
using D3DLab.Sandbox.App.Infrastructure;
using System.Collections.ObjectModel;
using D3DLab.App.Shell.Tabs;

namespace D3DLab.Sandbox.App.Presentation {
    internal class MainWindowViewModel : BaseNotify,
        IPluginHandler, ITopPanelViewModel, IEntityRenderSubscriber {
        readonly MainWindow mainWin;
        readonly DialogManager dialogs;
        readonly AppLogger logger;
        readonly PluginProxy plugins;
        readonly EngineNotificator notificator;
        readonly ContextStateProcessor context;
        WFScene? d3dScene;

        public ICommand HostLoadedCommand { get; }

        public ICommand OpenFilesCommand { get; }
        public ICommand CameraFocusToAllCommand { get; }
        public ICommand SaveAllCommand { get; }
        public ICommand OpenPluginsWindowCommand { get; }
        public BaseWPFCommand<bool> ShowWorldCoordinateSystemCommand { get; }
        public BaseWPFCommand<bool> ManipulatorToolEnabledCommand { get; }
        public GraphicsInfo GraphicsInfo { get; }
        public ObservableCollection<TabItemViewModel> RightTabs { get; }


        public MainWindowViewModel(MainWindow mainWin, DialogManager dialogs, AppLogger logger) {
            this.mainWin = mainWin;
            this.dialogs = dialogs;
            this.logger = logger;
            plugins = new PluginProxy(Path.Combine(AppContext.BaseDirectory, "plugins"));
            //
            GraphicsInfo = new GraphicsInfo();
            RightTabs = new ObservableCollection<TabItemViewModel>();
            //
            HostLoadedCommand = new WpfActionCommand<FormsHost>(OnHostLoaded);
            OpenPluginsWindowCommand = new WpfActionCommand(OnOpenPlugins);
            ShowWorldCoordinateSystemCommand = new WpfActionCommand<bool>(OnShowWorldCoordinateSystem);
            //

            notificator = new EngineNotificator();

            notificator.Subscribe(this);

            context = new ContextStateProcessor();
            context.AddState(0, x => GenneralContextState.Full(x,
                new AxisAlignedBox(new Vector3(-1000, -1000, -1000), new Vector3(1000, 1000, 1000)),
                notificator, logger));
            context.SwitchTo(0);

            plugins.Load();
        }


        #region d3d scene

        void OnHostLoaded(FormsHost host) {
            d3dScene = new WFScene(host, host.Overlay, context, notificator);
            d3dScene.Loaded += D3dScene_Loaded;
        }
        private void D3dScene_Loaded() {
            GraphicsInfo.Adapter = d3dScene.GetAdapterDescription().Description;
        }
        public override void OnUnloaded() {
            foreach (var plugin in plugins.Plugins) {
                plugin.Plugin.CloseAsync().Wait();
            }

            d3dScene.Dispose();

            notificator.Clear();

            context.Dispose();

            base.OnUnloaded();
        }

        void IEntityRenderSubscriber.Render(IEnumerable<GraphicEntity> entities) {
            var state = d3dScene.GetPerfomanceState();
            GraphicsInfo.Fps = state.FPS;
            GraphicsInfo.Milliseconds = state.ElapsedMilliseconds;
        }

        #endregion

        void OnOpenPlugins() {
            dialogs.Plugins.Open(vm => {
                vm.SetPluginProxy(plugins);
            });
        }

        void IPluginHandler.Handle(LoadedPlugin pl) {
            var plctx = new PluginContext(new PluginScene(context, d3dScene), pl.File.Directory, new PluginObservableCollection());

            if (!pl.IsResourcesLoaded) {
                try {
                    pl.Plugin.LoadResources(plctx);
                    pl.IsResourcesLoaded = true;
                } catch (Exception ex) {
                    logger.Error(ex);
                }
            }

            var vm = pl.Plugin.ExecuteAsComponent(plctx);

            RightTabs.AddTabWithPluginContent(pl.Plugin.Name, vm);

            mainWin.Dispatcher.InvokeAsync(() => {
                vm.Init();
            });

            dialogs.Plugins.Close();
        }


        void OnShowWorldCoordinateSystem(bool _checked) {
            if (_checked) {
                d3dScene.CoordinateSystem.Show(context);
            } else {
                d3dScene.CoordinateSystem.Hide(context);
            }
        }

    }
}
