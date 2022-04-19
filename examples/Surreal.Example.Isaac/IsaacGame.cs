using Isaac.Core.Actors;
using Isaac.Core.Dungeons;
using Isaac.Core.Systems;

namespace Isaac;

public sealed class IsaacGame : PrototypeGame
{
  public static Task Main() => StartAsync<IsaacGame>(new Configuration
  {
    Platform = new ConsolePlatform
    {
      Configuration =
      {
        Title          = "The Binding of Isaac",
        ShowFpsInTitle = true,
        FontSize       = 14,
      },
    },
  });

  public new IConsolePlatformHost Host    => (IConsolePlatformHost) base.Host;
  public     IConsoleDisplay      Display => Host.Display;

  public ActorScene? Scene { get; private set; }

  protected override void Initialize()
  {
    base.Initialize();

    Scene = new ActorScene(Services);

    Scene.Spawn(new Player());
    Scene.Spawn(new Dungeon(DungeonBlueprint.Simple));

    Scene.AddSystem(new PhysicsSystem());
    Scene.AddSystem(new GlyphSystem(Display));
    Scene.AddSystem(new TileMapSystem(Display));
  }

  protected override void BeginFrame(GameTime time)
  {
    base.BeginFrame(time);

    Scene?.BeginFrame(time.DeltaTime);
  }

  protected override void Input(GameTime time)
  {
    base.Input(time);

    Scene?.Input(time.DeltaTime);
  }

  protected override void Update(GameTime time)
  {
    base.Update(time);

    Scene?.Update(time.DeltaTime);
  }

  protected override void Draw(GameTime time)
  {
    base.Draw(time);

    Display.Fill(' ');

    Scene?.Draw(time.DeltaTime);
  }

  protected override void EndFrame(GameTime time)
  {
    base.EndFrame(time);

    Scene?.EndFrame(time.DeltaTime);
  }

  public override void Dispose()
  {
    Scene?.Dispose();

    base.Dispose();
  }
}
