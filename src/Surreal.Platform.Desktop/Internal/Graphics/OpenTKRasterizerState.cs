using OpenTK.Graphics.OpenGL;
using Surreal.Graphics;

namespace Surreal.Platform.Internal.Graphics;

internal sealed class OpenTKRasterizerState : IRasterizerState
{
  private Viewport viewport;
  private bool     isDepthTestingEnabled;
  private bool     isBlendingEnabled;

  public Viewport Viewport
  {
    get => viewport;
    set
    {
      viewport = value;
      GL.Viewport(viewport.X, viewport.Y, viewport.Width, viewport.Height);
    }
  }

  public bool IsDepthTestingEnabled
  {
    get => isDepthTestingEnabled;
    set
    {
      isDepthTestingEnabled = value;

      if (isDepthTestingEnabled)
      {
        GL.Enable(EnableCap.DepthTest);
      }
      else
      {
        GL.Disable(EnableCap.DepthTest);
      }
    }
  }

  public bool IsBlendingEnabled
  {
    get => isBlendingEnabled;
    set
    {
      isBlendingEnabled = value;

      if (isBlendingEnabled)
      {
        GL.Enable(EnableCap.Blend);
        GL.BlendFuncSeparate(
          sfactorRGB: BlendingFactorSrc.SrcAlpha,
          dfactorRGB: BlendingFactorDest.OneMinusSrcAlpha,
          sfactorAlpha: BlendingFactorSrc.SrcAlpha,
          dfactorAlpha: BlendingFactorDest.OneMinusSrcAlpha
        );
      }
      else
      {
        GL.Enable(EnableCap.Blend);
        GL.BlendFuncSeparate(
          sfactorRGB: BlendingFactorSrc.SrcColor,
          dfactorRGB: BlendingFactorDest.DstColor,
          sfactorAlpha: BlendingFactorSrc.SrcAlpha,
          dfactorAlpha: BlendingFactorDest.DstAlpha
        );
      }
    }
  }
}