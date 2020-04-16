using System.Numerics;
using Isaac.Core.Entities.Components;
using Surreal.Framework.Scenes.Entities;
using Surreal.Framework.Scenes.Entities.Aspects;
using Surreal.Framework.Scenes.Entities.Components;
using Surreal.Framework.Scenes.Entities.Systems;
using Surreal.Graphics.Cameras;
using Surreal.Input.Keyboard;
using Surreal.Input.Mouse;
using Surreal.Timing;

namespace Isaac.Core.Entities.Systems
{
  public sealed class InputSystem : IteratingSystem
  {
    public InputSystem(OrthographicCamera camera, IKeyboardDevice keyboard, IMouseDevice mouse)
      : base(Aspect.Of<Player, Transform>())
    {
      Camera   = camera;
      Keyboard = keyboard;
      Mouse    = mouse;
    }

    public OrthographicCamera Camera   { get; }
    public IKeyboardDevice    Keyboard { get; }
    public IMouseDevice       Mouse    { get; }

    public bool TrackPlayerMovements { get; set; } = false;

    protected override void Input(DeltaTime deltaTime, Entity entity)
    {
      if (!entity.IsPlayer()) return; // TODO: remove this once the aspects are working

      ref var player    = ref entity.Get<Player>();
      ref var transform = ref entity.Get<Transform>();

      var velocity = Vector2.Zero;

      if (Keyboard.IsKeyDown(Key.W)) velocity += Vector2.UnitY;
      if (Keyboard.IsKeyDown(Key.S)) velocity -= Vector2.UnitY;
      if (Keyboard.IsKeyDown(Key.A)) velocity -= Vector2.UnitX;
      if (Keyboard.IsKeyDown(Key.D)) velocity += Vector2.UnitX;

      transform.Position += velocity * player.MoveSpeed * deltaTime;

      if (TrackPlayerMovements)
      {
        Camera.Translate(new Vector3(velocity.X, velocity.Y, 0f));
      }
    }
  }
}
