using System;
using Surreal.Framework.Editing.Modes;
using Surreal.Platform;
using Surreal.Timing;

namespace Surreal.Framework
{
  public abstract class Editor<TGame> : IDisposable, IFrameListener
    where TGame : Game, new()
  {
    public static void Start<TEditor>(Configuration configuration)
      where TEditor : Editor<TGame>, new()
    {
      using var host   = configuration.Platform!.BuildHost();
      using var editor = new TEditor();

      editor.Initialize(host, configuration.Game);

      Engine.Run(host, editor);
    }

    public IPlatformHost Host { get; private set; }
    public TGame         Game { get; private set; }

    public EditorModeManager Modes { get; } = new EditorModeManager();

    internal void Initialize(IPlatformHost host, TGame game)
    {
      Host = host;
      Game = game;

      Initialize();
    }

    protected virtual void Initialize()
    {
      Host.Resized += OnResized;

      Game.Initialize(Host);
    }

    protected virtual void OnResized(int width, int height)
    {
    }

    public virtual void Tick(DeltaTime deltaTime)
    {
      Modes.Begin();

      Modes.Input(deltaTime);
      Modes.Update(deltaTime);
      Modes.Draw(deltaTime);

      Modes.End();
    }

    public void Dispose()
    {
      Modes.Dispose();
      Game.Dispose();
    }

    public sealed class Configuration
    {
      public IPlatform? Platform { get; set; }
      public TGame      Game     { get; set; } = new TGame();
    }
  }
}