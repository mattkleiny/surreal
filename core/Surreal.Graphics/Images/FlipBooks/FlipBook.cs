using Surreal.Assets;
using Surreal.Graphics.Images.Sprites;

namespace Surreal.Graphics.Images.FlipBooks;

/// <summary>A flip-book is a collection of named <see cref="SpriteAnimation"/>s.</summary>
public sealed class FlipBook
{
}

/// <summary>The <see cref="AssetLoader{T}"/> for <see cref="FlipBook"/>s.</summary>
public sealed class FlipBookLoader : AssetLoader<FlipBook>
{
  public override ValueTask<FlipBook> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    throw new NotImplementedException();
  }
}
