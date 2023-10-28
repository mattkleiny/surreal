using Surreal.Editing.Projects;
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

  private static async Task<AssetDatabase> BuildAssetDatabaseAsync()
  {
    var database = new AssetDatabase("Assets/Source", "Assets/Target")
    {
      Importers =
      {
        new BlueprintSchemaImporter()
      }
    };

    await database.RefreshAsync();

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
    public override async Task<BlueprintSchema> ImportAsync(VirtualPath path, CancellationToken cancellationToken = default)
    {
      await using var stream = await path.OpenInputStreamAsync();

      var schema = await JsonSerializer.DeserializeAsync<BlueprintSchema>(stream, cancellationToken: cancellationToken);
      if (schema == null)
      {
        throw new InvalidOperationException($"Unable to load blueprint schema from {path}");
      }

      return schema;
    }
  }
}
