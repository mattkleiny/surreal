﻿using Surreal.Graphics.Meshes;
using Surreal.Graphics.Textures;
using Surreal.Memory;
using Surreal.Resources;

namespace Surreal.Graphics;

/// <summary>
/// Base class for any graphical resource.
/// </summary>
public abstract class GraphicsAsset : TrackedAsset<GraphicsAsset>
{
  public static Size AllocatedBufferSize => GetSizeEstimate<GraphicsBuffer>();
  public static Size AllocatedTextureSize => GetSizeEstimate<Texture>();
  public static Size AllocatedMeshSize => GetSizeEstimate<Mesh>();
}
