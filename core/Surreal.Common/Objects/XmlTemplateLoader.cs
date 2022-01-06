using System.Reflection;
using Surreal.Assets;
using Surreal.IO;

namespace Surreal.Objects;

/// <summary>Loads <see cref="ITemplate{T}"/>s from XML.</summary>
public sealed class XmlTemplateLoader : AssetLoader<object>
{
  public override bool CanHandle(AssetLoaderContext context)
  {
    var isTemplate  = typeof(IImportableTemplate).IsAssignableFrom(context.AssetType);
    var hasTemplate = context.AssetType.GetCustomAttribute<TemplateAttribute>() != null;

    return isTemplate || hasTemplate;
  }

  public override async Task<object> LoadAsync(AssetLoaderContext context, CancellationToken cancellationToken = default)
  {
    // load template itself
    if (typeof(IImportableTemplate).IsAssignableFrom(context.AssetType))
    {
      await using var stream = await context.Path.OpenInputStreamAsync();

      return await TemplateFactory.ImportTemplateAsync(context.AssetType, stream, cancellationToken);
    }

    // instantiate template
    var templateType = TemplateFactory.GetTemplateType(context.AssetType);
    var template     = (ITemplate) await context.Manager.LoadAsset(templateType, context.Path, cancellationToken);

    return template.Create();
  }
}
