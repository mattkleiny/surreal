using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Surreal.Assets;

/// <summary>
/// An asset database is a collection of files and folders that are used to create a game.
/// </summary>
public class AssetDatabase
{
  public AssetDatabase(string sourcePath, string targetPath)
  {
    SourcePath = sourcePath;
    TargetPath = targetPath;
  }

  public string SourcePath { get; }
  public string TargetPath { get; }

  /// <summary>
  /// The <see cref="IAssetImporter"/>s that are used to import assets into the database.
  /// </summary>
  public List<IAssetImporter> Importers { get; } = new();

  /// <summary>
  /// Refreshes the asset database.
  /// </summary>
  public async Task RefreshAsync()
  {
    if (!Directory.Exists(SourcePath))
    {
      return;
    }

    var serializer = new SerializerBuilder()
      .WithNamingConvention(UnderscoredNamingConvention.Instance)
      .Build();

    foreach (var assetPath in Directory.GetFiles(SourcePath, "*.*", SearchOption.AllDirectories))
    {
      var resourceTypes = new List<Type>();

      foreach (var importer in Importers)
      {
        if (importer.CanHandle(assetPath))
        {
          resourceTypes.Add(importer.ResourceType);
        }
      }

      if (resourceTypes.Count == 0)
      {
        continue;
      }

      await using var stream = File.Open(Path.ChangeExtension(assetPath, "meta"), FileMode.Create);
      await using var writer = new StreamWriter(stream);

      var value = new
      {
        Id = Guid.NewGuid(),
        Resources = resourceTypes.Select(_ => _.FullName)
      };

      serializer.Serialize(writer, value);
    }
  }
}
