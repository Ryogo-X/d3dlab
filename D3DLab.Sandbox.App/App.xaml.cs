using System;
using System.Windows;

using D3DLab.ECS;
using WPFLab.Messaging;
using WPFLab;
using D3DLab.Sandbox.App.Presentation;
using D3DLab.Sandbox.App.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using D3DLab.App.Shell.Plugin;

namespace D3DLab.Sandbox.App {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class LabApp : LabApplication {

        protected override void ConfigureServices(IDependencyRegisterService registrator) {
            registrator
                .RegisterApplication(this)
                .RegisterAsSingleton<IMessenger, Messenger>()
                .RegisterUnhandledExceptionHandler()
                .Register<AppLogger>()
                .Register<IAppLogger>(x => x.GetService<AppLogger>())
                .Register<ILabLogger>(x => x.GetService<AppLogger>())
                //
                //.Register<AppSettings>()
                .RegisterView<MainWindow>()
                .Register<MainWindowViewModel>()
                //.Register<IFileLoader>(x => x.GetService<MainWindowViewModel>())
                //.Register<ISaveLoadedObject>(x => x.GetService<MainWindowViewModel>())
                .Register<IPluginHandler>(x => x.GetService<MainWindowViewModel>())
                //               
                //.RegisterMvvm()

                //dialogs 
                //    .RegisterDebugger()
                //    .RegisterTransient<OpenFilesViewModel>().RegisterTransientView<OpenFilesWindow>()
                //    .RegisterTransient<ObjDetailsViewModel>().RegisterTransientView<ObjDetailsWindow>()
                //.RegisterTransient<SaveAllViewModel>().RegisterTransientView<SaveAllWindow>()
                .RegisterTransient<PluginsViewModel>().RegisterTransientView<PluginsWindow>()
                .Register<Presentation.DialogManager>()
                ;
        }

        protected override void AppStartup(StartupEventArgs e, IDependencyResolverService resolver) {
            resolver.UseUnhandledExceptionHandler();
            resolver.ResolveView<MainWindow, MainWindowViewModel>().Show();
        }

        protected override void AppExit(ExitEventArgs e, IDependencyResolverService resolver) {
            resolver.RemoveUnhandledExceptionHandler();
        }
    }
}
