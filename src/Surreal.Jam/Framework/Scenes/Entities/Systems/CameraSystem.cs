using Surreal.Graphics;
using Surreal.Graphics.Cameras;
using Surreal.Platform;
using Surreal.Timing;

namespace Surreal.Framework.Scenes.Entities.Systems {
  public sealed class CameraSystem : EntitySystem {
    private readonly Camera        camera;
    private readonly IPlatformHost host;

    public CameraSystem(Camera camera, IPlatformHost host) {
      this.camera = camera;
      this.host   = host;

      host.Resized += OnHostResized;
    }

    public bool               ResizeAutomatically { get; set; }
    public ICameraController? Controller          { get; set; }

    public void OnHostResized(int width, int height) {
      if (ResizeAutomatically) {
        camera.Viewport = new Viewport(width, height);
      }
    }

    public override void Input(DeltaTime deltaTime) {
      Controller?.Input(deltaTime);
    }

    public override void Draw(DeltaTime deltaTime) {
      camera.Update();
    }

    public override void Dispose() {
      host.Resized -= OnHostResized;

      base.Dispose();
    }
  }
}