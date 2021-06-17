using System;
using Surreal.Data;

namespace Surreal.Assets {
  // TODO: abstract over assets with light-weight handles?
  // TODO: allow specifying parameters for assets when they're being loaded

  public sealed class Asset<T> : IDisposable {
    public Path         Path    { get; }
    public AssetManager Manager { get; }

    public bool IsReady => throw new NotImplementedException();

    internal Asset(Path path, AssetManager manager) {
      Path    = path;
      Manager = manager;
    }

    public static implicit operator T(Asset<T> asset) => throw new NotImplementedException();

    public void Dispose() {
      throw new NotImplementedException();
    }
  }
}