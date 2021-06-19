using System.Numerics;
using Asteroids.Components;
using Surreal.Framework;
using Surreal.Graphics.Sprites;

namespace Asteroids.Actors
{
  public class SpriteActor : Actor
  {
    public SpriteActor(IActorContext context, Sprite sprite)
        : base(context)
    {
      AddComponent(new TransformComponent
      {
        Position = Vector2.Zero,
        Rotation = 0f,
      });

      AddComponent(new SpriteComponent
      {
        Sprite = sprite
      });
    }

    public Vector2 Position
    {
      get => GetComponent<TransformComponent>().Position;
      set => GetComponent<TransformComponent>().Position = value;
    }

    public float Rotation
    {
      get => GetComponent<TransformComponent>().Rotation;
      set => GetComponent<TransformComponent>().Rotation = value;
    }

    public Sprite Sprite
    {
      get => GetComponent<SpriteComponent>().Sprite;
      set => GetComponent<SpriteComponent>().Sprite = value;
    }
  }
}