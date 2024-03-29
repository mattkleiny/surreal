﻿using Surreal.Assets;
using Surreal.Graphics.Sprites;
using Surreal.Services;

namespace Surreal;

/// <summary>
/// A <see cref="IServiceModule"/> for the Aseprite helpers.
/// </summary>
public sealed class AsepriteModule : IServiceModule
{
  public void RegisterServices(IServiceRegistry registry)
  {
    registry.AddService<IAssetLoader>(new AsepriteFileLoader());
    registry.AddService<IAssetLoader>(new AsepriteSpriteSheetLoader());
  }
}
