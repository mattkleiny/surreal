﻿using Surreal.Graphics.Textures;
using Surreal.IO;
using Surreal.Resources;

namespace Surreal.Graphics.Fonts;

/// <summary>
/// The <see cref="ResourceLoader{T}" /> for <see cref="BitmapFont" />s.
/// </summary>
public sealed class BitmapFontLoader : ResourceLoader<BitmapFont>
{
  public override async Task<BitmapFont> LoadAsync(ResourceContext context, CancellationToken cancellationToken)
  {
    var descriptor = await context.Path.DeserializeJsonAsync<BitmapFontDescriptor>(cancellationToken);

    var imagePath = GetImagePath(context.Path, descriptor);
    var texture = await context.LoadAsync<Texture>(imagePath, cancellationToken);

    return new BitmapFont(descriptor, texture);
  }

  private static VirtualPath GetImagePath(VirtualPath descriptorPath, BitmapFontDescriptor descriptor)
  {
    if (descriptor.FilePath != null)
    {
      if (Path.IsPathRooted(descriptor.FilePath))
      {
        return descriptor.FilePath;
      }

      return descriptorPath.GetDirectory().Resolve(descriptor.FilePath);
    }

    return descriptorPath.ChangeExtension("png");
  }
}