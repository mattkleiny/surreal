﻿using Surreal.Collections.Slices;
using Surreal.Graphics.Rendering;
using Surreal.Mathematics;

namespace Surreal.Scenes.Viewports;

/// <summary>
/// A camera that renders scene elements to the screen.
/// </summary>
public interface ICamera
{
  /// <summary>
  /// The frustum of the camera.
  /// </summary>
  ref readonly Frustum Frustum { get; }

  /// <summary>
  /// The projection view matrix of the camera.
  /// </summary>
  ref readonly Matrix4x4 ProjectionView { get; }
}

/// <summary>
/// A node that renders scene elements to the screen via a camera.
/// </summary>
public class CameraViewport : SceneNode, IRenderViewport
{
  private static readonly Matrix4x4 Identity = Matrix4x4.Identity;

  /// <summary>
  /// The active camera stack.
  /// </summary>
  public LinkedList<ICamera> ActiveCameras { get; } = [];

  /// <inheritdoc/>
  public ref readonly Matrix4x4 ProjectionView
  {
    get
    {
      if (!TryGetActiveCamera(out var camera))
      {
        return ref Identity;
      }

      return ref camera.ProjectionView;
    }
  }

  /// <summary>
  /// Tries to get the active viewport camera.
  /// </summary>
  public bool TryGetActiveCamera([MaybeNullWhen(false)] out ICamera camera)
  {
    // TODO: blend between the active cameras?
    camera = ActiveCameras.First?.Value;

    return camera != null;
  }

  /// <inheritdoc/>
  ReadOnlySlice<T> IRenderViewport.CullVisibleObjects<T>()
  {
    if (TryGetActiveCamera(out var camera))
    {
      return ResolveChildren<T>(instance =>
      {
        if (instance is ICullableObject value)
        {
          return value.IsVisibleToFrustum(camera.Frustum);
        }

        return true;
      });
    }

    return ReadOnlySlice<T>.Empty;
  }
}
