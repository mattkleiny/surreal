using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Materials.Shady.Backends;
using Surreal.IO;
using Surreal.Languages;

namespace Surreal.Graphics.Materials.Shady {
  public enum ShadyProgramType {
    Sprite,
    Mesh,
    Compute,
    Effect,
  }

  public sealed class ShadyMetadata {
    public string           Name        { get; }
    public string           Description { get; }
    public ShadyProgramType Type        { get; }

    public ShadyMetadata(string name, string description, ShadyProgramType type) {
      Name        = name;
      Description = description;
      Type        = type;
    }
  }

  [DebuggerDisplay("ShadyProgram {Metadata.Type} - {Metadata.Name}")]
  public sealed class ShadyProgram {
    public static async Task<ShadyProgram> LoadAsync(Path path) {
      var text   = await path.ReadAllTextAsync();
      var source = SourceText.FromString(text);

      return await ParseAsync(source);
    }

    public static Task<ShadyProgram> ParseAsync(SourceText source) {
      var parser   = new ShadyParser(source);
      var compiler = new ShadyCompiler(new GLSLCompilerBackend());

      var metadata = parser.MetadataDeclaration().Metadata;
      var shaders = parser.ShaderDeclarations()
          .Select(_ => compiler.Compile(metadata.Type, _.Type, _.Statements))
          .ToArray();

      var program = new ShadyProgram(source, metadata, shaders);

      return Task.FromResult(program);
    }

    public SourceText    Source   { get; }
    public ShadyMetadata Metadata { get; }
    public Shader[]      Shaders  { get; }

    public ShadyProgram(SourceText source, ShadyMetadata metadata, Shader[] shaders) {
      Source   = source;
      Metadata = metadata;
      Shaders  = shaders;
    }

    public ShaderProgram ToShaderProgram(IGraphicsDevice device) {
      return device.CreateShaderProgram(Shaders);
    }

    public sealed class Loader : AssetLoader<ShadyProgram> {
      public override async Task<ShadyProgram> LoadAsync(Path path, IAssetLoaderContext context) {
        return await ShadyProgram.LoadAsync(path);
      }
    }
  }
}