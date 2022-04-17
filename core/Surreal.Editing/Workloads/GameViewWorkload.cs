using System.Windows.Input;
using Avalonia.Controls;
using Surreal.Commands;
using Surreal.Utilities;

namespace Surreal.Workloads;

// Different commands for the game view workload
public readonly record struct EnterPlayModeCommand;
public readonly record struct ExitPlayModeCommand;
public readonly record struct TogglePlayModeCommand;

/// <summary>The <see cref="ViewModel"/> for the game view.</summary>
public sealed record GameViewModel : ViewModel
{
  private bool isPlaying;

  public bool IsPlaying
  {
    get => isPlaying;
    set => SetProperty(ref isPlaying, value);
  }

  public ICommand EnterPlayMode  { get; } = CommandHandlers.ToCommand(new EnterPlayModeCommand());
  public ICommand ExitPlayMode   { get; } = CommandHandlers.ToCommand(new ExitPlayModeCommand());
  public ICommand TogglePlayMode { get; } = CommandHandlers.ToCommand(new TogglePlayModeCommand());
}

/// <summary>A <see cref="Control"/> that hosts a view of the game inside a window.</summary>
public sealed class GameView : Control, IEditorHostControl
{
  public event Action<int, int>? Resized;

  public new int  Width     => (int) base.Width;
  public new int  Height    => (int) base.Height;
  public     bool IsClosing => false;

  protected override void OnInitialized()
  {
    base.OnInitialized();

    LayoutUpdated += OnLayoutUpdated;
  }

  private void OnLayoutUpdated(object? sender, EventArgs e)
  {
    Resized?.Invoke(Width, Height);
  }

  void IEditorHostControl.Blit()
  {
  }
}

/// <summary>The <see cref="EditorWorkload"/> that enables the <see cref="GameView"/>.</summary>
public sealed class GameViewWorkload : EditorWorkload, IServiceModule
{
  public GameViewWorkload(Game game)
  {
    Game = game;
  }

  public Game          Game      { get; }
  public GameViewModel ViewModel { get; } = new();

  public override IServiceModule Services => this;

  public override void Tick()
  {
    Game.Tick(); // update the game

    // TODO: blit the game to the game view bitmap?
  }

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    services.AddSingleton(Game);
    services.AddSingleton(ViewModel);

    services.AddCommandHandler<EnterPlayModeCommand>(OnEnterPlayMode);
    services.AddCommandHandler<ExitPlayModeCommand>(OnExitPlayMode);
    services.AddCommandHandler<TogglePlayModeCommand>(OnTogglePlayMode);
  }

  private void OnEnterPlayMode()
  {
    ViewModel.IsPlaying = true;
  }

  private void OnExitPlayMode()
  {
    ViewModel.IsPlaying = false;
  }

  private void OnTogglePlayMode()
  {
    ViewModel.IsPlaying = !ViewModel.IsPlaying;
  }
}
