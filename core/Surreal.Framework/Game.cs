﻿using Surreal.Graphics;
using Surreal.Resources;
using Surreal.Timing;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// A timing snapshot for the main game loop.
/// </summary>
public readonly record struct GameTime
{
  public required float DeltaTime { get; init; }
  public required float TotalTime { get; init; }
}

/// <summary>
/// Configuration for the game.
/// </summary>
public sealed record GameConfiguration
{
  /// <summary>
  /// The <see cref="IPlatform"/> to use for the game.
  /// </summary>
  public required IPlatform Platform { get; init; }

  /// <summary>
  /// The <see cref="IGameHost"/> for the game.
  /// </summary>
  public required IGameHost Host { get; init; }
}

/// <summary>
/// Static facade for the game integration.
/// </summary>
[ExcludeFromCodeCoverage]
public static class Game
{
  private static IGameHost? _host;

  /// <summary>
  /// The top-level <see cref="IGameHost"/> for the game.
  /// </summary>
  public static IGameHost Host
  {
    get => _host ?? throw new InvalidOperationException($"The {nameof(IGameHost)} has not been set.");
    set => _host = value;
  }

  /// <summary>
  /// The top-level <see cref="IEventBus"/> for the game.
  /// </summary>
  public static IEventBus Events => Host.Events;

  /// <summary>
  /// The top-level <see cref="IResourceProvider"/> for the game.
  /// </summary>
  public static IResourceProvider Resources => Host.Resources;

  /// <summary>
  /// The top-level <see cref="IServiceProvider"/> for the game.
  /// </summary>
  public static IServiceProvider Services => Host.Services;

  /// <summary>
  /// Starts a game on the given platform.
  /// </summary>
  public static void Start(GameConfiguration configuration)
  {
    using var platform = configuration.Platform.BuildHost();
    using var host = configuration.Host;

    platform.RegisterServices(host.Services);

    using var graphics = new GraphicsContext(host.Services.GetServiceOrThrow<IGraphicsBackend>());

    var clock = new DeltaTimeClock();
    var startTime = TimeStamp.Now;

    host.Initialize(graphics);

    while (!platform.IsClosing && !host.IsClosing)
    {
      var deltaTime = clock.Tick();
      var totalTime = TimeStamp.Now - startTime;

      var gameTime = new GameTime
      {
        DeltaTime = deltaTime,
        TotalTime = (float)totalTime.TotalSeconds
      };

      platform.BeginFrame(deltaTime);

      host.Tick(gameTime, graphics);

      platform.EndFrame(deltaTime);
    }
  }
}
