using System;
using Surreal.IO;

namespace Surreal.Content
{
  public readonly record struct AssetId(Type Type, Path Path)
  {
    public override string ToString() => Path.ToString();
  }
}
