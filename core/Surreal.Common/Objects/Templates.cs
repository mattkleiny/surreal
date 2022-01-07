using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Surreal.Objects;

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
    return Context.TryGetTemplateType(type, out _);
  }

  /// <summary>Gets the template associated with the given type.</summary>
  public static Type GetTemplateType(Type type)
  {
    if (!Context.TryGetTemplateType(type, out var templateType))
    {
      throw new InvalidOperationException($"The type {type} does not have a valid template associated");
    }

    return templateType;
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

  internal sealed class TemplateContext
  {
    private readonly Dictionary<Type, Type> templatesByType = new();

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
      templatesByType[type] = templateType;
    }

    public bool TryGetTemplateType(Type type, [MaybeNullWhen(false)] out Type result)
    {
      return templatesByType.TryGetValue(type, out result);
    }
  }
}
