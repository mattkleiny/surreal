using Surreal.Assets;
using Surreal.Objects;

namespace Surreal.IO.Json;

/// <summary>Loads templated objects from JSON.</summary>
public sealed class JsonAssetLoader : AssetLoader<object>
{
  public override bool CanHandle(AssetLoaderContext context)
  {
    return context.Path.Extension == ".json";
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
    return await context.Path.DeserializeJsonAsync(context.AssetType, progressToken.CancellationToken);
  }
}
