using System.Numerics;
using Surreal.Framework.Parameters;
using Surreal.Graphics.Sprites;
using Surreal.Graphics.Textures;
using Surreal.Mathematics;
using Surreal.Mathematics.Timing;
using Surreal.States;

namespace Isaac.Core.Mobs {
  public abstract class Mob : Actor {
    public         TextureRegion         Sprite    { get; }
    public         FSM<States>           State     { get; } = new FSM<States>();
    public virtual Vector2Parameter      Direction { get; } = new Vector2Parameter(Vector2.Zero);
    public virtual ClampedFloatParameter Speed     { get; } = new ClampedFloatParameter(4f, Range.Of(0f, 100f));
    public virtual ClampedIntParameter   Health    { get; } = new ClampedIntParameter(10, Range.Of(0, 100));
    public virtual AngleParameter        Rotation  { get; } = new AngleParameter(Angle.Zero);
    public virtual Vector2Parameter      Knockback { get; } = new Vector2Parameter(Vector2.Zero);

    public override bool IsEnabled => base.IsEnabled && State == States.Alive;
    public override bool IsVisible => base.IsVisible && State == States.Alive;

    protected Mob(TextureRegion sprite) {
      Sprite = sprite;
    }

    public override void Update(DeltaTime deltaTime) {
      base.Update(deltaTime);

      UpdatePosition(deltaTime);
      UpdateHealth();
    }

    private void UpdatePosition(DeltaTime deltaTime) {
      var velocity = Direction.Value * Speed * deltaTime;

      if (Knockback.Value.Length() > 0.2f) {
        velocity += Knockback.Value;

        Knockback.Value = Vector2.Lerp(Knockback, Vector2.Zero, deltaTime);
      } else {
        Knockback.Value = Vector2.Zero;
      }

      Position.Value += velocity;
    }

    private void UpdateHealth() {
      if (Health.Value <= 0) {
        State.ChangeState(States.Dead);
      }
    }

    public override void Draw(DeltaTime deltaTime) {
      base.Draw(deltaTime);

      Game.Current.SpriteBatch.DrawPivoted(
          region: Sprite,
          position: Position,
          pivot: Pivot.Center,
          rotation: Rotation,
          scale: 0.16f
      );
    }

    public enum States {
      Alive,
      Dead
    }
  }
}