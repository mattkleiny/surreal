using Surreal.Build.Assets;
using Surreal.IO;

namespace Surreal.Assets;

public class AssetDatabaseTests
{
  [Test]
  public async Task it_should_refresh_assets_from_disk()
  {
    var database = await BuildAssetDatabaseAsync();

    database.Assets.Count().Should().BeGreaterThan(0);
  }

  [Test]
  public async Task it_should_permit_querying_assets_by_id()
  {
    var database = await BuildAssetDatabaseAsync();

    database.GetAssetsById(new Guid("c323583d-9c75-4938-aec0-b6610ebb2bfc")).Should().NotBeNull();
  }

  [Test]
  public async Task it_should_permit_querying_assets_by_path()
  {
    var database = await BuildAssetDatabaseAsync();

    database.GetAssetsByPath("Assets/Source/blueprint01.json").Should().NotBeNull();
  }

  [Test]
  public async Task it_should_permit_querying_assets_by_type()
  {
    var database = await BuildAssetDatabaseAsync();

    database.GetAssetsByType(typeof(BlueprintSchema)).Should().NotBeEmpty();
  }

  [Test]
  public async Task it_should_import_all_assets_into_database()
  {
    var database = await BuildAssetDatabaseAsync();

    database.Assets.Should().Contain(it => it.AbsolutePath.EndsWith("blueprint01.json"));
    database.Assets.Should().NotContain(it => it.AbsolutePath.EndsWith("blueprint02.json"));

    await database.ImportAssetsAsync();

    database.Assets.Should().Contain(it => it.AbsolutePath.EndsWith("blueprint01.json"));
    database.Assets.Should().Contain(it => it.AbsolutePath.EndsWith("blueprint02.json"));
  }

  [Test]
  public async Task it_bake_assets()
  {
    var database = await BuildAssetDatabaseAsync();
    var baker = Substitute.For<IAssetBaker>();

    await database.BakeTargetAsync(baker, AssetBakingTarget.Desktop);
  }

  private static async Task<AssetDatabase> BuildAssetDatabaseAsync()
  {
    var database = new AssetDatabase("Assets/Source", "Assets/Target")
    {
      Importers =
      {
        new BlueprintSchemaImporter()
      }
    };

    await database.RefreshAssetsAsync();

    return database;
  }

  [AssetType("b7b60576-7d7c-41c9-a085-ef44b0e40706")]
  private sealed record BlueprintSchema
  {
    public required string Name { get; init; }

    public string? Description { get; init; }
  }

  private sealed class BlueprintSchemaImporter : AssetImporter<BlueprintSchema>
  {
    protected override bool CanHandlePath(string path)
    {
      return path.EndsWith(".json");
    }

    public override async Task<BlueprintSchema> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default)
    {
      await using var stream = path.OpenInputStream();

      var schema = await JsonSerializer.DeserializeAsync<BlueprintSchema>(stream, cancellationToken: cancellationToken);
      if (schema == null)
      {
        throw new InvalidOperationException($"Unable to load blueprint schema from {path}");
      }

      return schema;
    }
  }
}
