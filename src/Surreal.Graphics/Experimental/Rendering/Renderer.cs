using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Surreal.Collections;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Experimental.Rendering.Culling;

namespace Surreal.Graphics.Experimental.Rendering {
  public abstract class Renderer : IRenderer, IEnumerable<IRenderingPass>, IDisposable {
    private readonly MultiDictionary<RenderingStage, IRenderingPass> passesByStage
        = new MultiDictionary<RenderingStage, IRenderingPass>();

    private readonly IGraphicsDevice  device;
    private readonly ICullingStrategy cullingStrategy;
    private readonly CommandBuffer    commandBuffer;
    private readonly FrameBuffer      colorAttachment;
    private readonly FrameBuffer      depthAttachment;

    public Renderer(
        IGraphicsDevice device,
        ICullingStrategy cullingStrategy,
        in FrameBufferDescriptor colorDescriptor,
        in FrameBufferDescriptor depthDescriptor) {
      this.device          = device;
      this.cullingStrategy = cullingStrategy;

      commandBuffer   = device.Backend.CreateCommandBuffer();
      colorAttachment = device.Backend.CreateFrameBuffer(colorDescriptor);
      depthAttachment = device.Backend.CreateFrameBuffer(depthDescriptor);
    }

    public void Add(IRenderingPass pass)    => passesByStage.Add(pass.Stage, pass);
    public void Remove(IRenderingPass pass) => passesByStage.Remove(pass.Stage, pass);

    public virtual void RenderCamera(ICamera camera) {
      var cullingResults = cullingStrategy.PerformCulling(camera);

      var context = new RenderingContext(
          device: device,
          camera: camera,
          commands: commandBuffer,
          cullingResults: cullingResults,
          colorAttachment: colorAttachment,
          depthAttachment: depthAttachment
      );

      RenderCamera(ref context);
    }

    protected virtual void RenderCamera(ref RenderingContext context) {
      BeforeAll(ref context);

      RenderOpaque(ref context);
      RenderTransparent(ref context);
      RenderPostProcessing(ref context);

      AfterAll(ref context);

      BlitToScreen(ref context);
    }

    protected virtual void BeforeAll(ref RenderingContext context) {
      ExecuteStage(RenderingStage.BeforeAll, ref context);
    }

    protected virtual void RenderOpaque(ref RenderingContext context) {
      ExecuteStage(RenderingStage.BeforeOpaque, ref context);
      ExecuteStage(RenderingStage.AfterOpaque, ref context);
    }

    protected virtual void RenderTransparent(ref RenderingContext context) {
      ExecuteStage(RenderingStage.BeforeTransparent, ref context);
      ExecuteStage(RenderingStage.AfterTransparent, ref context);
    }

    protected virtual void RenderPostProcessing(ref RenderingContext context) {
      ExecuteStage(RenderingStage.BeforePostProcessing, ref context);
      ExecuteStage(RenderingStage.AfterPostProcessing, ref context);
    }

    protected virtual void AfterAll(ref RenderingContext context) {
      ExecuteStage(RenderingStage.AfterAll, ref context);
    }

    protected virtual void BlitToScreen(ref RenderingContext context) {
      context.Commands.Blit(
          device: context.Device,
          source: colorAttachment,
          target: device.Pipeline.PrimaryFrameBuffer
      );

      context.Commands.Flush();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected void ExecuteStage(RenderingStage stage, ref RenderingContext context) {
      var passes = passesByStage[stage];

      for (var i = 0; i < passes.Count; i++) {
        passes[i].Render(ref context);
      }
    }

    public void Dispose() {
      foreach (var pass in passesByStage.Values) {
        if (pass is IDisposable disposable) {
          disposable.Dispose();
        }
      }

      colorAttachment.Dispose();
      depthAttachment.Dispose();
      commandBuffer.Dispose();
    }

    public IEnumerator<IRenderingPass> GetEnumerator() => passesByStage.Values.GetEnumerator();
    IEnumerator IEnumerable.           GetEnumerator() => GetEnumerator();
  }
}