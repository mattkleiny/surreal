using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Objects.Templates;

/// <summary>Imports <see cref="ITemplate{T}"/>s from XML.</summary>
public static class XmlTemplateImporter
{
  public static async Task<TTemplate> ImportTemplateAsync<TTemplate>(string elementName, Stream stream, CancellationToken cancellationToken = default)
    where TTemplate : IImportableTemplate
  {
    var document = await XDocument.LoadAsync(stream, LoadOptions.None, cancellationToken);
    var element  = document.Element(elementName);

    if (element == null)
    {
      throw new XmlException($"Unable to parse {elementName} from the document");
    }

    var context  = new XmlImportContext(element);
    var template = Activator.CreateInstance<TTemplate>();

    template.OnImportTemplate(context);

    return template;
  }

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

/// <summary>Loads <see cref="ITemplate{T}"/>s from XML.</summary>
public sealed class XmlTemplateLoader<TTemplate> : AssetLoader<TTemplate>
  where TTemplate : IImportableTemplate
{
  private readonly string elementName;

  public XmlTemplateLoader(string elementName)
  {
    this.elementName = elementName;
  }

  public override async Task<TTemplate> LoadAsync(VirtualPath path, IAssetContext context, CancellationToken cancellationToken = default)
  {
    await using var stream = await path.OpenInputStreamAsync();

    return await XmlTemplateImporter.ImportTemplateAsync<TTemplate>(elementName, stream, cancellationToken);
  }
}
