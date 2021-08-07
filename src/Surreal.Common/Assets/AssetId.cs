using System;
using Surreal.IO;

namespace Surreal.Assets
{
  public readonly record struct AssetId(Type Type, Path Path)
  {
    public override string ToString() => Path.ToString();
  }
}
