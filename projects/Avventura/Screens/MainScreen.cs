using Avventura.Graphics;
using Avventura.Model.Actors;
using Surreal.Framework;
using Surreal.Framework.Scenes.Actors;
using Surreal.Framework.Scenes.Culling;
using Surreal.Framework.Screens;
using Surreal.Framework.Simulations;
using Surreal.Graphics.Cameras;
using Surreal.Graphics.Rendering;
using Surreal.Graphics.Textures;
using Surreal.Input.Keyboard;
using Surreal.Mathematics.Linear;

namespace Avventura.Screens
{
  public sealed class MainScreen : SimulationScreen<AvventuraGame, ActorSimulation>
  {
    private static readonly Vector2I Resolution = new Vector2I(256, 144);

    public OrthographicCamera Camera   { get; } = new OrthographicCamera(Resolution.X, Resolution.Y);
    public DedicatedRenderer  Renderer { get; private set; }

    public MainScreen(AvventuraGame game)
      : base(game)
    {
    }

    protected override ActorSimulation CreateSimulation()
    {
      var simulation = new ActorSimulation();

      simulation.Scene.Actors.Add(new Player()
      {
        Attributes =
        {
          Health  = {Value = 100},
          Toxin   = {Value = 100},
          Stamina = {Value = 10_000}
        }
      });

      return simulation;
    }

    public override void Initialize()
    {
      base.Initialize();

      Renderer = new DedicatedRenderer(
        device: GraphicsDevice,
        cullingStrategy: new SceneCullingStrategy<ActorScene>(Simulation.Scene),
        colorDescriptor: new FrameBufferDescriptor(
          width: Resolution.X,
          height: Resolution.Y,
          depth: 0,
          format: TextureFormat.RGBA8888,
          filterMode: TextureFilterMode.Point
        ),
        depthDescriptor: new FrameBufferDescriptor(
          width: Resolution.X,
          height: Resolution.Y,
          depth: 32,
          format: TextureFormat.RGBA8888,
          filterMode: TextureFilterMode.Point
        ),
        assets: Assets
      );

      Plugins.Add(new ScreenRenderingPlugin(Camera, Renderer));
    }

    public override void Input(GameTime time)
    {
      if (Keyboard.IsKeyPressed(Key.Escape)) Game.Exit();

      base.Input(time);
    }
  }
}
