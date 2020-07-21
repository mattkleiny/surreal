using System.Numerics;
using Surreal.Graphics.Cameras;
using Surreal.Mathematics.Timing;

namespace Isaac.Core {
  public sealed class CameraRig : Actor {
    public OrthographicCamera Camera { get; } = new OrthographicCamera(256 / 2, 144 / 2);

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      Camera.Position = new Vector3(Position.Value.X, Position.Value.Y, 0);
      Camera.Update();
    }
  }
}