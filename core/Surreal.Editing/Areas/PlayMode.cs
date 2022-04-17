using System.Windows.Input;
using Surreal.Commands;
using Surreal.Utilities;

namespace Surreal.Areas;

/// <summary>The <see cref="ViewModel"/> for play mode.</summary>
public sealed record PlayModeViewModel : ViewModel
{
  private bool isPlaying;

  public bool IsPlaying
  {
    get => isPlaying;
    set => SetProperty(ref isPlaying, value);
  }

  public ICommand EnterPlayMode  { get; } = EditorCommand.Create(new EnterPlayModeCommand());
  public ICommand ExitPlayMode   { get; } = EditorCommand.Create(new ExitPlayModeCommand());
  public ICommand TogglePlayMode { get; } = EditorCommand.Create(new TogglePlayModeCommand());
}

/// <summary>Puts the editor into play mode.</summary>
public readonly record struct EnterPlayModeCommand;

/// <summary>Exits the editor out of play mode.</summary>
public readonly record struct ExitPlayModeCommand;

/// <summary>Toggles the editor into/out of of play mode.</summary>
public readonly record struct TogglePlayModeCommand;

/// <summary>The <see cref="ICommandHandler{TCommand}"/> for play mode commands.</summary>
[RegisterService(typeof(ICommandHandler<EnterPlayModeCommand>))]
[RegisterService(typeof(ICommandHandler<ExitPlayModeCommand>))]
[RegisterService(typeof(ICommandHandler<TogglePlayModeCommand>))]
internal sealed class PlayModeCommandHandler : ICommandHandler<EnterPlayModeCommand>, ICommandHandler<ExitPlayModeCommand>, ICommandHandler<TogglePlayModeCommand>
{
  public ValueTask ExecuteAsync(EnterPlayModeCommand command, CancellationToken cancellationToken = default)
  {
    Editor.ViewModel.PlayMode.IsPlaying = true;

    return ValueTask.CompletedTask;
  }

  public ValueTask ExecuteAsync(ExitPlayModeCommand command, CancellationToken cancellationToken = default)
  {
    Editor.ViewModel.PlayMode.IsPlaying = false;

    return ValueTask.CompletedTask;
  }

  public ValueTask ExecuteAsync(TogglePlayModeCommand command, CancellationToken cancellationToken = default)
  {
    Editor.ViewModel.PlayMode.IsPlaying = !Editor.ViewModel.PlayMode.IsPlaying;

    return ValueTask.CompletedTask;
  }
}
