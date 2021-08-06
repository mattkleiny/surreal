using System.Numerics;
using Asteroids.Components;
using Surreal.Framework;

namespace Asteroids.Actors
{
  public class Actor : Surreal.Framework.Actor
  {
    public ref TransformComponent Transform => ref GetComponent<TransformComponent>();
    public ref SpriteComponent    Sprite    => ref GetComponent<SpriteComponent>();
    public ref HealthComponent    Health    => ref GetComponent<HealthComponent>();

    public Actor(IActorContext context)
        : base(context)
    {
      AddComponent(new TransformComponent
      {
        Position = Vector2.Zero,
        Rotation = 0f,
      });

      AddComponent(new SpriteComponent
      {
        Sprite = default,
      });

      AddComponent(new HealthComponent
      {
        Health = 100,
      });
    }
  }
}
