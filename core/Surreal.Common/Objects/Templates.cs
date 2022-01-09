using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Surreal.Collections;

namespace Surreal.Objects;

#pragma warning disable CA2255

/// <summary>A template for objects.</summary>
public interface ITemplate
{
  object Create();
}

/// <summary>A template for <see cref="T"/> instances.</summary>
public interface ITemplate<out T> : ITemplate
{
  new T Create();

  object ITemplate.Create()
  {
    return Create()!;
  }
}

/// <summary>Indicates the associated type is a <see cref="ITemplate{T}"/>.</summary>
[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class)]
public sealed class TemplateAttribute : Attribute
{
  public TemplateAttribute(Type type)
  {
    Type = type;
  }

  public Type Type { get; }
}

/// <summary>Static factory for <see cref="ITemplate{T}"/>s.</summary>
public static class TemplateFactory
{
  private static TemplateContext Context { get; } = new();

  /// <summary>Determines if the given type has a valid template associated with it.</summary>
  public static bool HasTemplate(Type type)
  {
    return Context.TryGetTemplateTypes(type, out _);
  }

  /// <summary>Gets the templates associated with the given type.</summary>
  public static ReadOnlySlice<Type> GetTemplateTypes(Type type)
  {
    if (!Context.TryGetTemplateTypes(type, out var templateTypes))
    {
      throw new InvalidOperationException($"The type {type} does not have a valid template associated");
    }

    return templateTypes;
  }

  /// <summary>Gets the template associated with the given type.</summary>
  public static Type GetTemplateType(Type type)
  {
    var templateTypes = GetTemplateTypes(type);

    if (templateTypes.Length > 1)
    {
      throw new InvalidOperationException($"There is more than one template associated with {type}");
    }

    return templateTypes[0];
  }

  /// <summary>Creates a blank instance of <see cref="T"/> from a default <see cref="ITemplate{T}"/> of it's type.</summary>
  public static T Create<T>()
  {
    return TemplateCache<T>.Template.Create();
  }

  /// <summary>Creates a blank <see cref="ITemplate{T}"/> for the given type.</summary>
  public static ITemplate<T> CreateTemplate<T>()
  {
    return (ITemplate<T>) CreateTemplate(typeof(T));
  }

  /// <summary>Creates a blank <see cref="ITemplate"/> for the given type.</summary>
  public static ITemplate CreateTemplate(Type type)
  {
    return (ITemplate) Activator.CreateInstance(GetTemplateType(type))!;
  }

  /// <summary>A static cache for <see cref="ITemplate{T}"/>s for <see cref="T"/>.</summary>
  private static class TemplateCache<T>
  {
    public static ITemplate<T> Template { get; } = CreateTemplate<T>();
  }

  [RequiresUnreferencedCode("Discovers templates via reflection")]
  internal sealed class TemplateContext
  {
    private readonly MultiDictionary<Type, Type> templatesByType = new();

    [ModuleInitializer]
    internal static void Initialize()
    {
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        Context.RegisterTemplates(assembly);
      }
    }

    private void RegisterTemplates(Assembly assembly)
    {
      var candidates =
        from type in assembly.GetTypes()
        from attribute in type.GetCustomAttributes<TemplateAttribute>(inherit: true)
        select new { Attribute = attribute, Type = type };

      foreach (var candidate in candidates)
      {
        RegisterTemplate(candidate.Attribute.Type, candidate.Type);
      }
    }

    private void RegisterTemplate(Type type, Type templateType)
    {
      templatesByType.Add(type, templateType);
    }

    public bool TryGetTemplateTypes(Type type, out ReadOnlySlice<Type> result)
    {
      return templatesByType.TryGetValues(type, out result);
    }
  }
}
