using Isaac.Mechanics;
using Isaac.Mechanics.Actors;
using Isaac.Mechanics.Effects;
using Surreal;
using Surreal.Mathematics;
using Surreal.Mechanics.Tactical;
using Surreal.Platform;
using Surreal.Timing;

namespace Isaac
{
  public sealed class Game : PrototypeGame
  {
    public static void Main() => Start<Game>(new()
    {
      Platform = new DesktopPlatform
      {
        Configuration =
        {
          Title          = "The Binding of Isaac",
          IsVsyncEnabled = true,
          ShowFPSInTitle = true,
        },
      },
    });

    private Actor actor;

    protected override void Initialize()
    {
      base.Initialize();

      GraphicsDevice.Pipeline.Rasterizer.IsBlendingEnabled = true;

      actor = new Actor
      {
        Health  = 100,
        Mana    = 10,
        Stamina = Normal.Max
      };

      actor.AddStatusEffect(new FrozenEffect());

      actor.AddStatusEffect(new DamageOverTimeEffect(
          Damage: new Damage(10, DamageTypes.Acid),
          Frequency: 1.Seconds(),
          Duration: 10.Seconds()
      ));
    }

    protected override void Update(GameTime time)
    {
      base.Update(time);

      actor.Update(time.DeltaTime);
    }
  }
}
