using System.Diagnostics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Materials;
using Surreal.IO;

namespace Surreal.Graphics.Experimental.Shady {
  [DebuggerDisplay("ShadyProgram {Metadata.Type} - {Metadata.Name}")]
  public sealed class ShadyProgram {
    public static async Task<ShadyProgram> LoadAsync(Path path) {
      var source = await path.ReadAllTextAsync();

      return await ParseAsync(source);
    }

    public static Task<ShadyProgram> ParseAsync(string source) {
      // TODO: actually parse the shader program into metadata and individual shaders
      var metadata = new ShadyMetadata(
          name: "Test Program",
          description: "A test shady program",
          type: ShadyProgramType.Effect
      );

      var shaders = new Shader[0];

      return Task.FromResult(new ShadyProgram(source, metadata, shaders));
    }

    public string        Source   { get; }
    public ShadyMetadata Metadata { get; }
    public Shader[]      Shaders  { get; }

    public ShadyProgram(string source, ShadyMetadata metadata, Shader[] shaders) {
      Source   = source;
      Metadata = metadata;
      Shaders  = shaders;
    }

    public ShaderProgram Compile(IGraphicsDevice device) {
      return device.CreateShaderProgram(Shaders);
    }

    public sealed class Loader : AssetLoader<ShadyProgram> {
      public override Task<ShadyProgram> LoadAsync(Path path, IAssetLoaderContext context) {
        return ShadyProgram.LoadAsync(path);
      }
    }
  }
}