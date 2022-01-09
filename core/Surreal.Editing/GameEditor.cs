using System.Windows;
using System.Windows.Threading;
using Surreal.Threading;

namespace Surreal;

/// <summary>Permits editing games with a simple in-situ game editor system.</summary>
public static class GameEditor
{
  /// <summary>Starts the editor and bootstraps the given <see cref="TGame"/>.</summary>
  public static Task StartAsync<TGame>(Game.Configuration configuration, CancellationToken cancellationToken = default)
    where TGame : Game, new()
  {
    var options = ThreadOptions.Default with
    {
      Name = "Editor Thread",
      IsBackground = true,

      // we need to use the STA model for Win32 controls and interop
      UseSingleThreadApartment = true,
    };

    return ThreadFactory.Create(options, async () =>
    {
      using var game       = Game.Create<TGame>(configuration);
      var       dispatcher = new WindowGameDispatcher(Dispatcher.CurrentDispatcher);

      await game.InitializeAsync(cancellationToken);

      dispatcher.Schedule(async () =>
      {
        await game.RunAsync(dispatcher, cancellationToken);

        Dispatcher.ExitAllFrames();
      });

      Dispatcher.Run();
    });
  }

  /// <summary>Shows a window in the dispatcher loop.</summary>
  public static void ShowWindow(Window window)
  {
    Dispatcher.CurrentDispatcher.BeginInvoke(window.Show);
  }

  private sealed class WindowGameDispatcher : IGameDispatcher
  {
    private readonly Dispatcher dispatcher;

    public WindowGameDispatcher(Dispatcher dispatcher)
    {
      this.dispatcher = dispatcher;
    }

    public void Schedule(Action continuation)
    {
      Dispatcher.CurrentDispatcher.BeginInvoke(continuation);
    }
  }
}
