using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Surreal.Templates;

/// <summary>Associates a <see cref="ITemplate{T}"/> with some type.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class TemplateAttribute : Attribute
{
  public Type Type { get; }

  public TemplateAttribute(Type type)
  {
    Type = type;
  }
}

/// <summary>Static factory for <see cref="ITemplate{T}"/>s and related instances.</summary>
public static class TemplateFactory
{
  /// <summary>Creates a blank instance of <see cref="T"/> from a default <see cref="ITemplate{T}"/> of it's type.</summary>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static T Create<T>() => TemplateCache<T>.Template.Create();

  /// <summary>Creates a blank <see cref="ITemplate{T}"/> for the given type.</summary>
  public static ITemplate<T> CreateTemplate<T>()
  {
    throw new NotImplementedException();
  }

  /// <summary>A static cache for <see cref="ITemplate{T}"/>s for <see cref="T"/>.</summary>
  private static class TemplateCache<T>
  {
    public static ITemplate<T> Template { get; } = CreateTemplate<T>();
  }
}
