﻿using D3DLab.ECS;
using D3DLab.ECS.Components;
using D3DLab.ECS.Context;
using D3DLab.ECS.Ext;
using D3DLab.ECS.Input;
using D3DLab.ECS.Systems;
using D3DLab.Render;
using D3DLab.Render.GameObjects;
using D3DLab.Std.Engine.Core.Common;
using D3DLab.Std.Engine.Core.Utilities;
using D3DLab.Viewer.D3D;
using D3DLab.Viewer.Debugger;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using System.Windows;
using WPFLab.MVVM;

namespace D3DLab.Viewer.Presentation.TDI.Scene {
    public class WFScene : D3DScene {
        BaseInputPublisher publisher;
        CurrentInputObserver input;
        bool isHandleCreated;

        public WFScene(ContextStateProcessor context, EngineNotificator notificator) : base(context, notificator) {
        }

        private void OnUnloaded(object sender, RoutedEventArgs e) {
            Dispose();
        }

        public void Init(System.Windows.Forms.Control control) {//object? sender, EventArgs e
            //publisher = new WPFInputPublisher(overlay); 
            publisher = new WinFormInputPublisher(control);
            input = new CurrentInputObserver(publisher);
            Window = new WFSurface(control, null, input);
            engine = new RenderEngine(Window, Context, notify);
            engine.Run(notify);

            isHandleCreated = true;

            InitScene();
        }

        public void InitContext() {
            var em = Context.GetEntityManager();

            var cameraTag = new ElementTag("CameraEntity");
            {//entities ordering 
                Context.EntityOrder
                       .RegisterOrder<RenderSystem>(cameraTag, 0)
                       .RegisterOrder<DefaultInputSystem>(cameraTag, 0);
            }

            CameraGameObject.Create(Context);
            LightGameObject.CreateAmbientLight(em);
            //  LightGameObject.CreatePointLight(em, Vector3.Zero + Vector3.UnitZ * 1000);
            LightGameObject.CreateDirectionLight(em, new Vector3(1, 4, 4).Normalized());

            var geo = GeometryBuilder.BuildGeoBox(new BoundingBox(new Vector3(-10, -10, -10), new Vector3(10, 10, 10)));

            EntityBuilders.BuildMeshElement(em, geo.Positions, geo.Indices, V4Colors.Blue)
                .UpdateComponent(TransformComponent.Create(Matrix4x4.CreateTranslation(new Vector3(-10, -10, -10))));

            EntityBuilders.BuildMeshElement(em, geo.Positions, geo.Indices, V4Colors.Red)
                .UpdateComponent(D3DLab.Toolkit.D3D.GeometryFlatShadingComponent.Create());//.UpdateAlfa(0.3f)

            EntityBuilders.BuildMeshElement(em, geo.Positions, geo.Indices, V4Colors.Green)//.UpdateAlfa(0.5f)
                .UpdateComponent(TransformComponent.Create(Matrix4x4.CreateTranslation(new Vector3(10, 10, 10))));
        }

        public override void Dispose() {
            //host.Loaded -= OnHandleCreated;
            // host.Unloaded -= OnUnloaded;

            engine.Dispose();
            publisher.Dispose();
            input?.Dispose();

            base.Dispose();
        }

        internal void ReCreate(System.Windows.Forms.Control control) {
            engine.Dispose();
            publisher.Dispose();
            input?.Dispose();

            publisher = new WinFormInputPublisher(control);
            input = new CurrentInputObserver(publisher);
            Window = new WFSurface(control, null, input);
            engine = new RenderEngine(Window, Context, notify);

            foreach (var com in Context.GetComponentManager().GetComponents<IRenderableComponent>()) {
                com.ClearBuffers();
            }

            foreach (var sys in Context.GetSystemManager().GetSystems<RenderSystem>()) {
                sys.Init(engine.Graphics);
            }

            engine.Run(notify);

            //InitScene();
        }
    }

    public class SceneViewModel : BaseNotify {

        public WFScene Scene { get; private set; }

        public IContextState Context => context;

        ContextStateProcessor context;
        EngineNotificator notificator;
        readonly IDockingManager dockingManager;
        FormsHost host;

        public SceneViewModel(IDockingManager dockingManager) {
            this.dockingManager = dockingManager;

            dockingManager.OpenSceneTab(this);
        }

        public void SetContext(ContextStateProcessor context, EngineNotificator notificator) {
            this.context = context;
            this.notificator = notificator;

        }
        public void SetSurfaceHost(FormsHost host) {
            this.host = host;
            host.SurfaceCreated += SurfaceCreated;
        }


        public void SurfaceCreated(System.Windows.Forms.Control obj) {
            if (Scene != null) {
                Scene.ReCreate(obj);
                return;
            }
            
            Scene =  new WFScene(context, notificator);
            Scene.Init(obj);
            Scene.InitContext();
        }


    }
}
