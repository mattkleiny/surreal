[![Build, Test and Package](https://github.com/mattkleiny/surreal/actions/workflows/build-master.yml/badge.svg)](https://github.com/mattkleiny/surreal/actions/workflows/build-master.yml)

# Surreal

A simple hobby game engine built with modern .NET technologies.

## About The Project

Surreal is a simple game engine built with the most recent .NET tech and C# features.
It is designed mainly as a learning tool for game engine architecture and design.

It's similar in scope to existing game engines like [MonoGame](https://www.monogame.net/) and [libGDX](https://libgdx.com/),
with an emphasis on simplicity and ease of use.

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet)

### Getting Started

1. Clone the repository ```git clone https://github.com/mattkleiny/surreal.git```
2. Build the project ```dotnet build```
3. Run the tests ```dotnet test```
4. Run an example ```dotnet run --project ./examples/HelloWorld```

## Usage

The following code snippet show how to use Surreal to create a simple game loop.

```csharp
// configure the platform for hosting the game
var configuration = new GameConfiguration
{
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, Surreal!",
      IsVsyncEnabled = true,
      ShowFpsInTitle = true,
      Width = 1920,
      Height = 1080
    }
  }
};

// request access to whatever services you need in the start method
Game.Start(configuration, (Game game, IGraphicsBackend graphics, IKeyboardDevice keyboard) =>
{
  var color1 = Random.Shared.Next<Color>();
  var color2 = Random.Shared.Next<Color>();

  // start a game loop and execute a callback on every frame
  game.ExecuteVariableStep(time =>
  {
    var color = Color.Lerp(color1, color2, MathE.PingPong(time.TotalTime));

    graphics.ClearColorBuffer(color);

    if (keyboard.IsKeyPressed(Key.Escape))
    {
      game.Exit();
    }
  });
});
```
