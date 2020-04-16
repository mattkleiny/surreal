using System;
using System.Numerics;
using Surreal.Graphics.Cameras;

namespace Surreal.UI
{
  public delegate Matrix4x4 StageViewport();

  public static class StageViewports
  {
    private const float NearPlane = 0.1f;
    private const float FarPlane  = 1000f;

    public static StageViewport Fixed(float width, float height) => Fixed(Matrix4x4.CreateOrthographic(width, height, NearPlane, FarPlane));
    public static StageViewport Fixed(Matrix4x4 projectionView)  => () => projectionView;

    public static StageViewport Overlay(ICamera camera) => () => camera.ProjectionView;
    public static StageViewport Fill(ICamera camera)    => throw new NotImplementedException();
    public static StageViewport Stretch(ICamera camera) => throw new NotImplementedException();
    public static StageViewport Scale(ICamera camera)   => throw new NotImplementedException();
    public static StageViewport Extend(ICamera camera)  => throw new NotImplementedException();
  }
}