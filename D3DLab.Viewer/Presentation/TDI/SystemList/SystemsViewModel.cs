﻿using D3DLab.ECS;
using D3DLab.ECS.Shaders;
using D3DLab.Viewer.Debugger;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using WPFLab.Messaging;
using WPFLab.MVVM;

namespace D3DLab.Viewer.Presentation.TDI.SystemList {
    public class SystemItemViewModel : BaseNotify {
        readonly IGraphicSystem system;

        public string Header { get; }
        public bool IsShaderEditable { get; }

        public TimeSpan ExecutionTime => system.ExecutionTime;

        public SystemItemViewModel(IGraphicSystem system) {
            this.system = system;
            Header = system.ToString().Split('.').Last();
            IsShaderEditable = system is IShadersContainer;
        }

        public IGraphicSystem GetOriginSystem() {
            return system;
        }

        public void RefreshExecutingTime() {
            SetPropertyChanged(nameof(ExecutionTime));
        }
    }

    public class SystemsViewModel : BaseNotify {

        #region commands

        ICommand openShaderEditor;
        public ICommand OpenShaderEditor {
            get => openShaderEditor;
            private set {
                Update(ref openShaderEditor, value);
            }
        }

        public ICommand OpenPropertiesEditor { get; }
        public class OpenShaderEditorSystemItemCommand : OpenShaderEditorCommand<SystemItemViewModel> {
            public OpenShaderEditorSystemItemCommand(IDockingManager docking, IRenderUpdater updater) : base(docking, updater) { }
            protected override IShadersContainer Convert(SystemItemViewModel i) {
                return (IShadersContainer)i.GetOriginSystem();
            }
        }
        class OpenPropertiesEditorSystemItemCommand : OpenPropertiesEditorCommand<SystemItemViewModel> {
            public class EditingPropertiesComponentItem : IEditingProperties {
                readonly SystemItemViewModel item;

                public string Titile => item.Header;
                public object TargetObject => item.GetOriginSystem();

                public EditingPropertiesComponentItem(SystemItemViewModel item) {
                    this.item = item;
                }

                public void MarkAsModified() {
                    throw new NotImplementedException();
                }
            }

            public OpenPropertiesEditorSystemItemCommand(IDockingManager updater) : base(updater) { }

            protected override IEditingProperties Convert(SystemItemViewModel item) {
                return new EditingPropertiesComponentItem(item);
            }
        }

        #endregion

        public int Count => items.Count;
        public ICollectionView Items { get; }
        readonly ObservableCollection<SystemItemViewModel> items;
        readonly DispatcherTimer timer;
        readonly IDockingManager dockingManager;
        IRenderUpdater updater;

        public SystemsViewModel(IDockingManager dockingManager) {
            this.dockingManager = dockingManager;
            items = new ObservableCollection<SystemItemViewModel>();
            Items = CollectionViewSource.GetDefaultView(items);

            OpenPropertiesEditor = new OpenPropertiesEditorSystemItemCommand(dockingManager);

            Items.CurrentChanged += OnCurrentChanged;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += OnTimeRefresh;
            timer.Start();

            dockingManager.OpenSystemsTab(this);
        }

        public void SetCurrentRenderUpdater(IRenderUpdater updater) {
            this.updater = updater;

            OpenShaderEditor = new OpenShaderEditorSystemItemCommand(dockingManager, updater);
        }

        private void OnTimeRefresh(object sender, EventArgs e) {
            RefreshExecutingTime();
        }

        private void OnCurrentChanged(object sender, EventArgs e) {

        }

        public void AddSystem(IGraphicSystem system) {
            var item = new SystemItemViewModel(system);
            AddItem(item);
        }

        public void Update() {
            //GameWindow.InputManager.PushCommand(new Std.Engine.Core.Input.Commands.ForceRenderCommand());
        }

        void AddItem(SystemItemViewModel item) {
            items.Add(item);
        }

        void RefreshExecutingTime() {
            foreach (var i in items) {
                i.RefreshExecutingTime();
            }
        }
    }
}
