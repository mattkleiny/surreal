using System.Composition;
using JetBrains.Annotations;
using Surreal.Timing;

namespace Surreal;

/// <summary>Exports a <see cref="IMod"/> for use in game composition.</summary>
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

/// <summary>Provides metadata about a <see cref="IMod"/>.</summary>
public interface IModMetadata
{
  string? Name        { get; }
  string? Description { get; }
  string? Version     { get; }
}

/// <summary>A mod for a game.</summary>
public interface IMod : IDisposable
{
  void Initialize(IModRegistry registry);
  void Input(DeltaTime deltaTime);
  void Update(DeltaTime deltaTime);
  void Draw(DeltaTime deltaTime);
}

/// <summary>Base class for any <see cref="IMod"/> implementation.</summary>
public abstract class Mod : IMod
{
  public virtual void Initialize(IModRegistry registry)
  {
  }

  public virtual void Input(DeltaTime deltaTime)
  {
  }

  public virtual void Update(DeltaTime deltaTime)
  {
  }

  public virtual void Draw(DeltaTime deltaTime)
  {
  }

  public virtual void Dispose()
  {
  }
}
