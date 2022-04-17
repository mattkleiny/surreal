using Surreal.Assets;

namespace Civilization.Core;

public sealed record RuleSet
{
  public ImmutableDictionary<string, Building>        Buildings        { get; init; } = ImmutableDictionary<string, Building>.Empty;
  public ImmutableDictionary<string, Nation>          Nations          { get; init; } = ImmutableDictionary<string, Nation>.Empty;
  public ImmutableDictionary<string, Terrain>         Terrains         { get; init; } = ImmutableDictionary<string, Terrain>.Empty;
  public ImmutableDictionary<string, TileImprovement> TileImprovements { get; init; } = ImmutableDictionary<string, TileImprovement>.Empty;
  public ImmutableDictionary<string, TileResource>    TileResources    { get; init; } = ImmutableDictionary<string, TileResource>.Empty;
  public ImmutableDictionary<string, Unit>            Units            { get; init; } = ImmutableDictionary<string, Unit>.Empty;
}

public sealed class RuleSetLoader : AssetLoader<RuleSet>
{
  public override ValueTask<RuleSet> LoadAsync(AssetLoaderContext context, ProgressToken progressToken = default)
  {
    throw new NotImplementedException();
  }
}
