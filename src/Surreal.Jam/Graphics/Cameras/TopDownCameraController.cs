using System.Numerics;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Timing;

namespace Surreal.Graphics.Cameras {
  public sealed class TopDownCameraController : ICameraController {
    private readonly OrthographicCamera camera;
    private readonly IKeyboardDevice    keyboard;

    public TopDownCameraController(OrthographicCamera camera, IKeyboardDevice keyboard) {
      this.camera   = camera;
      this.keyboard = keyboard;
    }

    public float Speed     { get; set; } = 100f;
    public float ZoomSpeed { get; set; } = 0.1f;

    public void Input(DeltaTime deltaTime) {
      // transform camera position relative to world's cardinal axes
      if (keyboard.IsKeyDown(Key.W)) camera.Translate(Vector3.UnitY * Speed  * deltaTime);
      if (keyboard.IsKeyDown(Key.S)) camera.Translate(-Vector3.UnitY * Speed * deltaTime);
      if (keyboard.IsKeyDown(Key.A)) camera.Translate(-Vector3.UnitX * Speed * deltaTime);
      if (keyboard.IsKeyDown(Key.D)) camera.Translate(Vector3.UnitX * Speed  * deltaTime);

      // zoom orthographic projection in/out
      if (keyboard.IsKeyDown(Key.Q)) camera.Zoom += ZoomSpeed * Speed * deltaTime;
      if (keyboard.IsKeyDown(Key.E)) camera.Zoom -= ZoomSpeed * Speed * deltaTime;
    }
  }
}