﻿using D3DLab.ECS;
using D3DLab.ECS.Camera;
using D3DLab.ECS.Components;
using D3DLab.SDX.Engine;
using D3DLab.SDX.Engine.Rendering;
using D3DLab.Toolkit._CommonShaders;
using D3DLab.Toolkit.Components;

using SharpDX.Direct2D1;
using SharpDX.Direct3D11;
using System;
using System.Linq;
using System.Numerics;

namespace D3DLab.Toolkit.Render {

    public class RenderSystem : D3DRenderSystem<ToolkitRenderProperties> {
        CameraState prevCameraState;

        SharpDX.Direct3D11.Buffer gameDataBuffer;
        SharpDX.Direct3D11.Buffer lightDataBuffer;

        public RenderSystem() {
            prevCameraState = new CameraState() {
                ViewMatrix = Matrix4x4.Identity,
                ProjectionMatrix = Matrix4x4.Identity,
                LookDirection = -Vector3.UnitZ,
                Position = Vector3.Zero
            };
        }

        protected override void UpdateBuffers(GraphicsDevice device) {
            //camera
            var gamebuff = GameStructBuffer.FromCameraState(prevCameraState, device.Size);
            gameDataBuffer = device.CreateBuffer(BindFlags.ConstantBuffer, ref gamebuff);

            //lights
            var dinamicLightbuff = new LightStructBuffer[3];
            lightDataBuffer = device.CreateDynamicBuffer(dinamicLightbuff,
                LightStructBuffer.Size * dinamicLightbuff.Length);
        }

        protected override void Executing(ISceneSnapshot snapshot) {
            var emanager = ContextState.GetEntityManager();
            var ticks = (float)snapshot.FrameRateTime.TotalMilliseconds;

            Synchronize();
            var registrator = new RenderTechniqueRegistrator<ToolkitRenderProperties>(nested);
            try {
                using (var frame = graphics.FrameBegin()) {

                    foreach (var entity in emanager.GetEntities().OrderBy(x => x.GetOrderIndex<RenderSystem>())) {
                        if (entity.Has(typeof(RenderableComponent), typeof(TransformComponent))) {
                            registrator.Register(entity);
                        }
                    }

                    prevCameraState = snapshot.Camera;
                    var lights = snapshot.Lights.Select(x => LightStructBuffer.From(x)).ToArray();
                    var gamebuff = GameStructBuffer.FromCameraState(prevCameraState, snapshot.Window.Size);

                    frame.Graphics.UpdateSubresource(ref gamebuff, gameDataBuffer);
                    frame.Graphics.UpdateDynamicBuffer(lights, lightDataBuffer);

                    foreach (var str in registrator.Techniques) {
                        if (str is IGraphicSystemContextDependent dependent) {
                            dependent.ContextState = ContextState;
                        }
                        str.Render(frame.Graphics, new ToolkitRenderProperties(gameDataBuffer, lightDataBuffer, prevCameraState));
                    }
                }
            } catch (SharpDX.CompilationException cex) {
                System.Diagnostics.Trace.WriteLine($"CompilationException[\n{cex.Message.Trim()}]");
            } catch (SharpDX.SharpDXException shex) {
                //var reason = frame.Graphics.D3DDevice.DeviceRemovedReason;
                System.Diagnostics.Trace.WriteLine(shex.Message);
                throw shex;
            } catch (Exception ex) {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                throw ex;
            } finally {
                registrator.Cleanup();
                Pass = registrator.Techniques.SelectMany(x => x.GetPass()).ToArray();
            }
        }
    }
}