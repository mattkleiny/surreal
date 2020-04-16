using System;
using System.Composition;
using JetBrains.Annotations;

namespace Surreal
{
  [MeansImplicitUse]
  [MetadataAttribute]
  [AttributeUsage(AttributeTargets.Class)]
  public sealed class ExportModAttribute : ExportAttribute, IModMetadata
  {
    public ExportModAttribute()
      : base(typeof(IMod))
    {
    }

    public string? Name        { get; set; }
    public string? Description { get; set; }
    public string? Version     { get; set; }
  }
}
