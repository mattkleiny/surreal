using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Graphics.Materials.Shady;
using Surreal.Graphics.Meshes;
using Surreal.IO;
using Surreal.Mathematics.Linear;
using Surreal.States;

namespace Surreal.Graphics.Materials {
  public abstract class ShaderProgram : GraphicsResource {
    public abstract void Bind(VertexAttributes attributes);

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

    private sealed class HotLoadingShaderProgram : ShaderProgram {
      private readonly IGraphicsDevice device;
      private readonly IPathWatcher    watcher;

      public FSM<States>    State   { get; } = new FSM<States>();
      public Path           Path    { get; }
      public ShaderProgram? Program { get; private set; }

      public HotLoadingShaderProgram(IGraphicsDevice device, Path path) {
        this.device = device;

        Path = path;

        watcher = path.Watch();

        watcher.Modified += OnPathModified;
        watcher.Deleted  += OnPathDeleted;
      }

      private void OnPathModified(Path path) {
        State.ChangeState(States.Dirty);

        ShadyProgram.LoadAsync(path).ContinueWith(previous => {
          Program = previous.Result.Compile(device);
          State.ChangeState(States.Ready);
        });
      }

      private void OnPathDeleted(Path path) {
        State.ChangeState(States.Invalid);
      }

      public override void Bind(VertexAttributes attributes) {
        CheckValidity();
        Program!.Bind(attributes);
      }

      public override void SetUniform(string alias, int scalar) {
        CheckValidity();
        Program!.SetUniform(alias, scalar);
      }

      public override void SetUniform(string alias, float scalar) {
        CheckValidity();
        Program!.SetUniform(alias, scalar);
      }

      public override void SetUniform(string alias, Vector2I point) {
        CheckValidity();
        Program!.SetUniform(alias, point);
      }

      public override void SetUniform(string alias, Vector3I point) {
        CheckValidity();
        Program!.SetUniform(alias, point);
      }

      public override void SetUniform(string alias, Vector2 vector) {
        CheckValidity();
        Program!.SetUniform(alias, vector);
      }

      public override void SetUniform(string alias, Vector3 vector) {
        CheckValidity();
        Program!.SetUniform(alias, vector);
      }

      public override void SetUniform(string alias, Vector4 vector) {
        CheckValidity();
        Program!.SetUniform(alias, vector);
      }

      public override void SetUniform(string alias, Quaternion quaternion) {
        CheckValidity();
        Program!.SetUniform(alias, quaternion);
      }

      public override void SetUniform(string alias, in Matrix2x2 matrix) {
        CheckValidity();
        Program!.SetUniform(alias, in matrix);
      }

      public override void SetUniform(string alias, in Matrix3x2 matrix) {
        CheckValidity();
        Program!.SetUniform(alias, in matrix);
      }

      public override void SetUniform(string alias, in Matrix4x4 matrix) {
        CheckValidity();
        Program!.SetUniform(alias, in matrix);
      }

      protected override void Dispose(bool managed) {
        if (managed) {
          watcher.Dispose();
        }

        base.Dispose(managed);
      }

      [Conditional("DEBUG")]
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      private void CheckValidity() {
        if (State == States.Invalid || Program == null) {
          throw new Exception($"The shader program at {Path} is no longer valid");
        }
      }

      public enum States {
        Dirty,
        Ready,
        Invalid,
      }
    }

    public sealed class Loader : AssetLoader<ShaderProgram> {
      private readonly IGraphicsDevice device;
      private readonly bool            hotReloading;

      public Loader(IGraphicsDevice device, bool hotReloading) {
        this.device       = device;
        this.hotReloading = hotReloading;
      }

      public override async Task<ShaderProgram> LoadAsync(Path path, IAssetLoaderContext context) {
        if (hotReloading && path.GetFileSystem().SupportsWatcher) {
          return new HotLoadingShaderProgram(device, path);
        }

        var input  = await context.GetAsync<ShadyProgram>(path);
        var output = input.Compile(device);

        return output;
      }
    }
  }
}