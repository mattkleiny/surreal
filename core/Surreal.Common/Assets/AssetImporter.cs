namespace Surreal.Assets;

/// <summary>An importer for assets of type <see cref="T"/>.</summary>
public abstract class AssetImporter<T> : AssetLoader<T>
  where T : notnull
{
  private readonly ImmutableHashSet<string> extensions;

  protected AssetImporter(params string[] extensions)
  {
    this.extensions = extensions.ToImmutableHashSet();
  }

  public override bool CanHandle(AssetLoaderContext context)
  {
    return base.CanHandle(context) && extensions.Contains(context.Path.Extension);
  }
}
