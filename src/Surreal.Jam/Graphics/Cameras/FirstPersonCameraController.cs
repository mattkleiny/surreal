using System.Numerics;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Timing;

namespace Surreal.Graphics.Cameras
{
  public sealed class FirstPersonCameraController : ICameraController
  {
    private readonly PerspectiveCamera camera;
    private readonly IKeyboardDevice   keyboard;
    private readonly IMouseDevice      mouse;

    public FirstPersonCameraController(PerspectiveCamera camera, IKeyboardDevice keyboard, IMouseDevice mouse)
    {
      this.camera   = camera;
      this.keyboard = keyboard;
      this.mouse    = mouse;
    }

    public float Speed           { get; set; } = 5f;
    public float DegreesPerPixel { get; set; } = 0.5f;

    public void Input(DeltaTime deltaTime)
    {
      var velocity = Speed * deltaTime;

      if (keyboard.IsKeyDown(Key.LeftShift)) velocity *= 2f;

      // transform camera position relative to camera's cardinal axes
      if (keyboard.IsKeyDown(Key.W)) camera.Translate(camera.Forward  * velocity);
      if (keyboard.IsKeyDown(Key.S)) camera.Translate(-camera.Forward * velocity);
      if (keyboard.IsKeyDown(Key.A)) camera.Translate(-camera.Right   * velocity);
      if (keyboard.IsKeyDown(Key.D)) camera.Translate(camera.Right    * velocity);
      if (keyboard.IsKeyDown(Key.Q)) camera.Translate(camera.Up       * velocity);
      if (keyboard.IsKeyDown(Key.E)) camera.Translate(-camera.Up      * velocity);

      // rotate camera direction relative to mouse delta
      var mouseDelta = mouse.DeltaPosition;
      if (mouseDelta.X != 0 || mouseDelta.Y != 0)
      {
        var deltaX = -mouseDelta.X * DegreesPerPixel * deltaTime; // left/right or yaw
        var deltaY = -mouseDelta.Y * DegreesPerPixel * deltaTime; // up/down or pitch

        // normalized rotation about pitch and yaw delta
        camera.Direction = Vector3.Transform(camera.Direction, Matrix4x4.CreateFromAxisAngle(camera.Right, deltaY));
        camera.Direction = Vector3.Transform(camera.Direction, Matrix4x4.CreateFromAxisAngle(camera.Up, deltaX));
      }
    }
  }
}
