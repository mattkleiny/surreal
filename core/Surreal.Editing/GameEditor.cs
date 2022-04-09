using System.Windows;
using System.Windows.Threading;
using Surreal.Internal;
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
      using var game = Game.Create<TGame>(configuration);
      var dispatcher = new WindowsDispatcher(Dispatcher.CurrentDispatcher);

      await game.InitializeAsync(cancellationToken);

#pragma warning disable CS4014
      // TODO: clean this up?
      Dispatcher.CurrentDispatcher.BeginInvoke(async () =>
      {
        await game.RunAsync(dispatcher, cancellationToken);

        Dispatcher.ExitAllFrames();
      });
#pragma warning restore CS4014

      Dispatcher.Run();
    });
  }

  /// <summary>Shows a window in the dispatcher loop.</summary>
  public static void ShowWindow(Window window)
  {
    Dispatcher.CurrentDispatcher.BeginInvoke(window.Show);
  }
}
