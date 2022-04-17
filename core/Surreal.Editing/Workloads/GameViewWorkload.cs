using System.Windows.Input;
using Avalonia.Controls;
using Surreal.Commands;
using Surreal.Utilities;

namespace Surreal.Workloads;

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

/// <summary>Puts the editor into play mode.</summary>
public readonly record struct EnterPlayModeCommand;

/// <summary>Exits the editor out of play mode.</summary>
public readonly record struct ExitPlayModeCommand;

/// <summary>Toggles the editor into/out of of play mode.</summary>
public readonly record struct TogglePlayModeCommand;

/// <summary>A <see cref="Control"/> that hosts a view of the game.</summary>
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
  public override IServiceModule Services => this;

  void IServiceModule.RegisterServices(IServiceRegistry services)
  {
    var viewModel = new GameViewModel();

    services.AddSingleton(viewModel);
    services.AddSingleton(CommandHandlers.Create<EnterPlayModeCommand>(() => viewModel.IsPlaying = true));
    services.AddSingleton(CommandHandlers.Create<ExitPlayModeCommand>(() => viewModel.IsPlaying = false));
    services.AddSingleton(CommandHandlers.Create<ToggleSwitch>(() => viewModel.IsPlaying = !viewModel.IsPlaying));
  }
}
