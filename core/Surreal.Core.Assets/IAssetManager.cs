using System.Collections.Generic;
using Surreal.Data;

namespace Surreal.Assets {
  public interface IAssetManager : IAssetResolver, IEnumerable<object> {
    int Count { get; }

    bool Contains<TAsset>(Path path);
  }
}