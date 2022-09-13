using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

using D3DLab.ECS;
using D3DLab.ECS.Components;
using D3DLab.Toolkit;
using D3DLab.Toolkit.Components;
using D3DLab.Viewer.D3D.Systems;

namespace D3DLab.App.Shell.D3D.Systems {
    public struct ZoomToObjectComponent : IGraphicComponent {
        public static ZoomToObjectComponent Create() =>
            new ZoomToObjectComponent(ElementTag.New());

        public ElementTag Tag { get; }
        public bool IsValid => true;
        public ZoomToObjectComponent(ElementTag elementTag) : this() {
            this.Tag = elementTag;
        }
        public void Dispose() {
            // Method intentionally left empty.
        }
    }

    class ZoomToObjectSystem : BaseEntitySystem, IGraphicSystem, IGraphicSystemContextDependent {
        public IContextState? ContextState { set; private get; }

        protected override void Executing(ISceneSnapshot snapshot) {
            if (ContextState == null) {
                throw new InvalidOperationException("ContextState can't be null");
            }

            var emanager = ContextState.GetEntityManager();

            foreach (var entity in emanager.GetEntities()) {
                if (entity.TryGetComponents<GeometryBoundsComponent, ZoomToObjectComponent, RenderableComponent>
                    (out var box, out var zoom, out var renderable) && renderable.IsRenderable) {

                    var camera = ContextState.GetEntityManager().GetEntity(snapshot.CurrentCameraTag);

                    var com = CameraUtils.FocusCameraOnBox(camera.GetComponent<OrthographicCameraComponent>(),
                        box.Bounds, snapshot.Surface.Size);

                    camera.UpdateComponent(com);

                    entity.RemoveComponent<ZoomToObjectComponent>();
                    
                    break;
                }
            }
        }
    }
}
