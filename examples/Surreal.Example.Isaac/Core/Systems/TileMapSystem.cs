﻿using Isaac.Core.Actors.Components;
using Surreal.Components;
using Surreal.Graphics.Meshes;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Permits rendering tile maps in the world.</summary>
public sealed class TileMapSystem : IteratingSystem
{
  private readonly SpriteBatch batch;

  public TileMapSystem(SpriteBatch batch)
    : base(ComponentMask.Of<Transform, Sprite>())
  {
    this.batch = batch;
  }
}
