using System.Numerics;
using Surreal.Graphics.Cameras;
using Surreal.Mathematics.Timing;

namespace Isaac.Core {
  public sealed class CameraRig : Actor {
    public OrthographicCamera Camera { get; } = new(viewportWidth: 256 / 2, viewportHeight: 144 / 2);

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Camera.Position = new Vector3(Position.Value.X, Position.Value.Y, 0);
      Camera.Update();
    }
  }
}