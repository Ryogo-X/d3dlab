using D3DLab.ECS;
using D3DLab.ECS.Components;
using D3DLab.Toolkit;
using D3DLab.Toolkit.Components;

using SharpDX.Direct3D9;

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace D3DLab.Viewer.D3D.Systems {
    public struct ZoomToAllCompponent : IGraphicComponent {
        public static ZoomToAllCompponent Create() =>
            new ZoomToAllCompponent(ElementTag.New());

        public ElementTag Tag { get; }
        public bool IsValid => true;
        public ZoomToAllCompponent(ElementTag elementTag) : this() {
            this.Tag = elementTag;
        }
        public void Dispose() {
            // Method intentionally left empty.
        }
    }
    public class ZoomToAllObjectsSystem : BaseEntitySystem, IGraphicSystem, IGraphicSystemContextDependent {
        public IContextState? ContextState { set; private get; }

        protected override void Executing(ISceneSnapshot snapshot) {
            if (ContextState == null) {
                throw new InvalidOperationException("ContextState can't be null");
            }

            var emanager = ContextState.GetEntityManager();

            var world = emanager.GetEntity(snapshot.WorldTag);

            if (!world.Contains<ZoomToAllCompponent>()) {
                return;
            }

            var fullBox = new AxisAlignedBox();
            foreach (var entity in emanager.GetEntities()) {
                if (entity.TryGetComponents<GeometryBoundsComponent, TransformComponent, RenderableComponent>
                    (out var box, out var tr, out var renderable) && renderable.IsRenderable) {
                    fullBox = fullBox.Merge(box.Bounds.Transform(tr.MatrixWorld));
                }
            }

            var camera = ContextState.GetEntityManager().GetEntity(snapshot.CurrentCameraTag);
            var com = CameraUtils.FocusCameraOnBox(camera.GetComponent<OrthographicCameraComponent>(),
                      fullBox, snapshot.Surface.Size);

            camera.UpdateComponent(com);

            world.RemoveComponent<ZoomToAllCompponent>();
        }
    }
}
