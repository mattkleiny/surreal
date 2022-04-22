using Avventura.Core.Actors;

namespace Avventura;

public sealed class AvventuraGame : PrototypeGame
{
  public static void Main() => Start<AvventuraGame>(new Configuration
  {
    Platform = new ConsolePlatform
    {
      Configuration =
      {
        Title          = "Avventura",
        ShowFpsInTitle = true,
        FontSize       = 14,
      },
    },
  });

  public new IConsolePlatformHost Host    => (IConsolePlatformHost) base.Host;
  public     IConsoleDisplay      Display => Host.Display;

  public ActorScene? Scene { get; private set; }

  protected override void OnInitialize()
  {
    base.OnInitialize();

    Scene = new ActorScene(Services);

    Scene.Spawn(new Player
    {
      Position = new Point2(16, 16),
    });
  }

  protected override void OnBeginFrame(GameTime time)
  {
    base.OnBeginFrame(time);

    Scene?.BeginFrame(time.DeltaTime);
  }

  protected override void OnInput(GameTime time)
  {
    base.OnInput(time);

    Scene?.Input(time.DeltaTime);
  }

  protected override void OnUpdate(GameTime time)
  {
    base.OnUpdate(time);

    Scene?.Update(time.DeltaTime);
  }

  protected override void OnDraw(GameTime time)
  {
    base.OnDraw(time);

    Display.Fill(' ');

    Scene?.Draw(time.DeltaTime);
  }

  protected override void OnEndFrame(GameTime time)
  {
    base.OnEndFrame(time);

    Scene?.EndFrame(time.DeltaTime);
  }

  public override void Dispose()
  {
    Scene?.Dispose();

    base.Dispose();
  }
}
