using Surreal.Assets;
using Surreal.Objects;

namespace Surreal.IO.Xml;

/// <summary>Loads templated objects from XML.</summary>
public sealed class XmlAssetLoader : AssetLoader<object>
{
  public override bool CanHandle(AssetLoaderContext context)
  {
    return context.Path.Extension == ".xml";
  }

  public override async ValueTask<object> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    // instantiate template
    if (!typeof(ITemplate).IsAssignableFrom(context.AssetType))
    {
      var templateType = TemplateFactory.GetTemplateType(context.AssetType);
      var template = (ITemplate) await context.Manager.LoadAssetAsync(templateType, context.Path);

      return template.Create();
    }

    // load template itself
    return await context.Path.DeserializeXmlAsync(context.AssetType, progressToken.CancellationToken);
  }
}
