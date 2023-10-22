﻿using Surreal.Colors;
using Surreal.Graphics.Textures;

namespace Surreal.Graphics.Rendering;

/// <summary>
/// A lightweight forward rendering pipeline.
/// </summary>
[RenderPipeline("Forward")]
public sealed class ForwardRenderPipeline : MultiPassRenderPipeline
{
  public ForwardRenderPipeline(IGraphicsBackend backend)
    : base(backend)
  {
    Passes.Add(new ColorBufferPass(backend));
    Passes.Add(new ShadowStencilPass(backend));
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects color data.
  /// </summary>
  private sealed class ColorBufferPass(IGraphicsBackend backend) : RenderPass
  {
    private readonly RenderTarget _colorTarget = new(backend, new RenderTargetDescriptor
    {
      // TODO: make this dynamic
      Width = 1920,
      Height = 1080,
      Format = TextureFormat.Rgba8,
      FilterMode = TextureFilterMode.Point,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthStencilFormat = DepthStencilFormat.None,
    });

    public override void OnBeginPass(in RenderFrame frame)
    {
      _colorTarget.Bind();

      _colorTarget.ClearColorBuffer(Color.Clear);
    }

    public override void OnExecutePass(in RenderFrame frame)
    {
      foreach (var renderObject in frame.VisibleObjects)
      {
        renderObject.Render(in frame);
      }
    }

    public override void OnEndPass(in RenderFrame frame)
    {
      _colorTarget.Unbind();
    }

    public override void OnEndFrame(in RenderFrame frame)
    {
      // TODO: blit the color target to the back buffer
    }

    public override void Dispose()
    {
      _colorTarget.Dispose();

      base.Dispose();
    }
  }

  /// <summary>
  /// A <see cref="RenderPass"/> that collects stenciled shadow map data.
  /// </summary>
  private sealed class ShadowStencilPass(IGraphicsBackend backend) : RenderPass
  {
    private readonly RenderTarget _shadowTarget = new(backend, new RenderTargetDescriptor
    {
      // TODO: make this dynamic
      Width = 1920,
      Height = 1080,
      Format = TextureFormat.R8,
      FilterMode = TextureFilterMode.Point,
      WrapMode = TextureWrapMode.ClampToEdge,
      DepthStencilFormat = DepthStencilFormat.Depth24Stencil8
    });

    public override void OnBeginPass(in RenderFrame frame)
    {
      _shadowTarget.Bind();

      _shadowTarget.ClearColorBuffer(Color.Black);
      _shadowTarget.ClearStencilBuffer(1);
    }

    public override void OnExecutePass(in RenderFrame frame)
    {
      foreach (var renderObject in frame.VisibleObjects)
      {
        renderObject.Render(in frame);
      }
    }

    public override void OnEndPass(in RenderFrame frame)
    {
      _shadowTarget.Unbind();
    }


    public override void Dispose()
    {
      _shadowTarget.Dispose();

      base.Dispose();
    }
  }
}