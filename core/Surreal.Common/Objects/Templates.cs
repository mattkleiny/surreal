using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
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

/// <summary>A <see cref="ITemplate{T}"/> that can be imported.</summary>
public interface IImportableTemplate : ITemplate
{
  void OnImportTemplate(ITemplateImportContext context);
}

/// <summary>A <see cref="ITemplate{T}"/> that can be imported.</summary>
public interface IImportableTemplate<out T> : IImportableTemplate, ITemplate<T>
{
}

/// <summary>A context for <see cref="ITemplate{T}"/> imports.</summary>
public interface ITemplateImportContext
{
  T Parse<T>(string key, T defaultValue = default!);
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
  /// <summary>Gets the template associated with the given type.</summary>
  public static Type GetTemplateType(Type type)
  {
    if (typeof(ITemplate).IsAssignableFrom(type))
    {
      return type;
    }

    var attribute = type.GetCustomAttribute<TemplateAttribute>();
    if (attribute == null || !typeof(ITemplate).IsAssignableFrom(attribute.Type))
    {
      throw new InvalidOperationException($"The type {type} does not have a valid template associated");
    }

    return attribute.Type;
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

  /// <summary>Imports a template for the given type from the given XML stream.</summary>
  public static async Task<ITemplate<T>> ImportTemplateAsync<T>(Stream stream, CancellationToken cancellationToken)
  {
    return (ITemplate<T>) await ImportTemplateAsync(typeof(T), stream, cancellationToken);
  }

  /// <summary>Imports a template for the given type from the given XML stream.</summary>
  public static async Task<ITemplate> ImportTemplateAsync(Type type, Stream stream, CancellationToken cancellationToken)
  {
    var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
    var element  = document.Elements().SingleOrDefault(); // expect a single top-level element

    if (element == null)
    {
      throw new XmlException("Expected a single top-level element");
    }

    var context  = new XmlImportContext(element);
    var template = CreateTemplate(type);

    if (template is not IImportableTemplate importable)
    {
      throw new InvalidOperationException($"The template {template.GetType()} is not importable");
    }

    importable.OnImportTemplate(context);

    return template;
  }

  /// <summary>A static cache for <see cref="ITemplate{T}"/>s for <see cref="T"/>.</summary>
  private static class TemplateCache<T>
  {
    public static ITemplate<T> Template { get; } = CreateTemplate<T>();
  }

  /// <summary>A <see cref="ITemplateImportContext"/> for XML parsing.</summary>
  private sealed class XmlImportContext : ITemplateImportContext
  {
    private readonly XElement element;

    public XmlImportContext(XElement element)
    {
      this.element = element;
    }

    public T Parse<T>(string key, T defaultValue = default!)
    {
      if (this.element.Attribute(key) is { Value: var attribute })
      {
        return (T) Convert.ChangeType(attribute, typeof(T), CultureInfo.InvariantCulture);
      }

      if (this.element.Element(key) is { Value: var element })
      {
        return (T) Convert.ChangeType(element, typeof(T), CultureInfo.InvariantCulture);
      }

      return defaultValue;
    }
  }
}
