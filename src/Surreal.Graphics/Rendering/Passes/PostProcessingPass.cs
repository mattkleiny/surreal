using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Assets;
using Surreal.Collections;
using Surreal.Graphics.Rendering.PostProcessing;

namespace Surreal.Graphics.Rendering.Passes
{
  public class PostProcessingPass : IRenderingPass, IEnumerable<IPostProcessingEffect>, IDisposable
  {
    private readonly MultiDictionary<PostProcessingEffectStage, IPostProcessingEffect> effectsByStage =
      new MultiDictionary<PostProcessingEffectStage, IPostProcessingEffect>();

    private readonly ShaderFactory shaderFactory;
    private readonly FrameBuffer  buffer1;
    private readonly FrameBuffer  buffer2;

    public PostProcessingPass(IGraphicsDevice device, IAssetResolver assets, FrameBufferDescriptor bufferDescriptor)
    {
      shaderFactory = new ShaderFactory(assets);

      buffer1 = device.Factory.CreateFrameBuffer(bufferDescriptor);
      buffer2 = device.Factory.CreateFrameBuffer(bufferDescriptor);
    }

    RenderingStage IRenderingPass.Stage => RenderingStage.BeforePostProcessing;

    public void Add(IPostProcessingEffect pass)    => effectsByStage.Add(pass.Stage, pass);
    public void Remove(IPostProcessingEffect pass) => effectsByStage.Remove(pass.Stage, pass);

    public virtual void Render(ref RenderingContext renderingContext)
    {
      var context = new PostProcessingContext(
        renderingContext: renderingContext,
        sourceBuffer: buffer1,
        targetBuffer: buffer2,
        shaderFactory: shaderFactory
      );

      BeforeAll(ref context);
      RenderEffects(ref context);
      AfterAll(ref context);

      BlitToScreen(ref context);
    }

    protected virtual void BeforeAll(ref PostProcessingContext context)
    {
      ExecuteStage(PostProcessingEffectStage.BeforeAll, ref context);
    }

    protected virtual void RenderEffects(ref PostProcessingContext context)
    {
      ExecuteStage(PostProcessingEffectStage.EarlyEffects, ref context);
      ExecuteStage(PostProcessingEffectStage.StandardEffects, ref context);
      ExecuteStage(PostProcessingEffectStage.LateEffects, ref context);
    }

    protected virtual void AfterAll(ref PostProcessingContext context)
    {
      ExecuteStage(PostProcessingEffectStage.AfterAll, ref context);
    }

    protected virtual void BlitToScreen(ref PostProcessingContext context)
    {
      context.Commands.Blit(
        device: context.Device,
        source: context.SourceBuffer,
        target: context.ColorAttachment
      );
    }

    protected void ExecuteStage(PostProcessingEffectStage stage, ref PostProcessingContext context)
    {
      var passes = effectsByStage[stage];

      for (var i = 0; i < passes.Count; i++)
      {
        passes[i].Render(ref context);

        context = context.SwapBuffers();
      }
    }

    public void Dispose()
    {
      foreach (var effect in effectsByStage.Values)
      {
        if (effect is IDisposable disposable)
        {
          disposable.Dispose();
        }
      }

      buffer1.Dispose();
      buffer2.Dispose();
    }

    public IEnumerator<IPostProcessingEffect> GetEnumerator() => effectsByStage.Values.GetEnumerator();
    IEnumerator IEnumerable.                  GetEnumerator() => GetEnumerator();
  }
}