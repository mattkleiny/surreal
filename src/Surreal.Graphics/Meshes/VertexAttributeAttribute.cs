using System;
using JetBrains.Annotations;

namespace Surreal.Graphics.Meshes {
  [AttributeUsage(AttributeTargets.Field), MeansImplicitUse]
  public sealed class VertexAttributeAttribute : Attribute {
    public string     Alias      { get; set; } = string.Empty;
    public int        Count      { get; set; }
    public VertexType Type       { get; set; }
    public bool       Normalized { get; set; }
  }
}