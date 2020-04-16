using System.Collections.Generic;
using Surreal.IO;

namespace Surreal.Assets
{
  public interface IAssetManager : IAssetResolver, IEnumerable<object>
  {
    int Count { get; }

    bool Contains<TAsset>(Path path);
  }
}
