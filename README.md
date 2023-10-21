[![Build, Test and Package](https://github.com/mattkleiny/surreal/actions/workflows/build-and-package.yml/badge.svg)](https://github.com/mattkleiny/surreal/actions/workflows/build-and-package.yml)

# Surreal

A simple game engine built with modern .NET technologies.

## About The Project

Surreal is a simple game engine built with the most recent .NET tech and C# features.
It is designed mainly as a learning tool for game engine architecture and design.

It's similar in scope to existing game engines like [MonoGame](https://www.monogame.net/) and
[libGDX](https://libgdx.com/), but is much more limited in features and performance.

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet)

### Installation

1. Clone the repository ```sh git clone https://github.com/mattkleiny/surreal.git```
2. Build the project ```sh dotnet build```
3. Run the tests ```sh dotnet test```
4. Run an example ```sh dotnet run --project ./examples/HelloWorld```

## Usage

The following code snippets show how to use Surreal to create a simple game.

### Creating a Game

```csharp
Game.Start(new GameConfiguration
{
  // swap and configure the particular platform you want to target
  Platform = new DesktopPlatform
  {
    Configuration =
    {
      Title = "Hello, World!",
      Width = 1920,
      Height = 1080
    }
  },
  Host = GameHost.Create(() =>
  {
    // do one time initialization here, and access the game services
    var graphics = Game.Services.GetServiceOrThrow<IGraphicsBackend>();

    var color1 = Random.Shared.Next<Color>();
    var color2 = Random.Shared.Next<Color>();

    // return a function that will be called every frame
    return time =>
    {
      var color = Color.Lerp(color1, color2, MathE.PingPong(time.TotalTime));

      graphics.ClearColorBuffer(color);
    };
  })
});
```
