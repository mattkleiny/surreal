using Asteroids.Actors.Components;
using Surreal.Graphics.Textures;
using Surreal.States;

namespace Asteroids.Actors
{
  public sealed class Ship : AsteroidActor
  {
    public FSM<States> State { get; } = new FSM<States>(States.Alive);

    public int Health { get; set; } = 10;
    public int Damage { get; set; } = 2;

    public Ship(TextureRegion sprite)
    {
      Components.Add(new SpriteComponent(this)
      {
        Sprite = sprite
      });

      Components.Add(new InputComponent(this));
    }

    public enum States
    {
      Alive,
      Dead
    }
  }
}
