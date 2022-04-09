using Isaac.Core.Actors;
using Surreal.Input.Keyboard;
using Surreal.Systems;

namespace Isaac.Core.Systems;

/// <summary>Allows the <see cref="IKeyboardDevice"/> to control a <see cref="IPawn"/>.</summary>
public sealed class KeyboardSystem : SceneSystem
{
  private readonly IPawn pawn;
  private readonly IKeyboardDevice keyboard;

  private Vector2 direction;

  public KeyboardSystem(IPawn pawn, IKeyboardDevice keyboard)
  {
    this.pawn = pawn;
    this.keyboard = keyboard;
  }

  public override void OnInput(DeltaTime deltaTime)
  {
    direction = Vector2.Zero;

    if (keyboard.IsKeyDown(Key.W)) direction.Y += 1;
    if (keyboard.IsKeyDown(Key.S)) direction.Y -= 1;
    if (keyboard.IsKeyDown(Key.A)) direction.X -= 1;
    if (keyboard.IsKeyDown(Key.D)) direction.X += 1;
  }

  public override void OnUpdate(DeltaTime deltaTime)
  {
    if (direction.LengthSquared() > 0f)
    {
      pawn.Move(direction);
    }
  }
}
