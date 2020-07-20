using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Meshes;
using Surreal.IO;
using Surreal.Languages;
using Surreal.Mathematics.Linear;

namespace Surreal.Graphics.Materials.Shaders {
  public enum ShaderProgramType {
    Sprite,
    Screen,
  }

  public abstract class ShaderProgram : GraphicsResource {
    public abstract void Bind(VertexAttributeSet attributes);

    public abstract void SetUniform(string alias, int scalar);
    public abstract void SetUniform(string alias, float scalar);
    public abstract void SetUniform(string alias, Vector2I point);
    public abstract void SetUniform(string alias, Vector3I point);
    public abstract void SetUniform(string alias, Vector2 vector);
    public abstract void SetUniform(string alias, Vector3 vector);
    public abstract void SetUniform(string alias, Vector4 vector);
    public abstract void SetUniform(string alias, Quaternion quaternion);
    public abstract void SetUniform(string alias, in Matrix2x2 matrix);
    public abstract void SetUniform(string alias, in Matrix3x2 matrix);
    public abstract void SetUniform(string alias, in Matrix4x4 matrix);

    public sealed class Loader : AssetLoader<ShaderProgram> {
      private readonly IGraphicsDevice device;
      private readonly IShaderCompiler compiler;
      private readonly bool            hotReloading;

      public Loader(IGraphicsDevice device, IShaderCompiler compiler, bool hotReloading) {
        this.device       = device;
        this.compiler     = compiler;
        this.hotReloading = hotReloading;
      }

      public override async Task<ShaderProgram> LoadAsync(Path path, IAssetLoaderContext context) {
        if (hotReloading && path.GetFileSystem().SupportsWatcher) {
          throw new NotSupportedException("Hot reloading is not yet implemented!");
        }

        var source   = await context.GetAsync<SourceText>(path);
        var parser   = new ShaderParser(source);
        var metadata = parser.MetadataDeclaration();
        var uniforms = parser.UniformDeclarations();
        var shaders  = parser.ShaderDeclarations();

        var results = shaders.Select(shader => compiler.Compile(
            programType: metadata.Type,
            shaderType: shader.Type,
            uniforms: uniforms,
            functions: new[] {shader.Function}
        ));

        return device.CreateShaderProgram(results.ToArray());
      }
    }
  }
}