[![Build, Test and Package](https://github.com/mattkleiny/surreal/actions/workflows/build-and-package.yml/badge.svg)](https://github.com/mattkleiny/surreal/actions/workflows/build-and-package.yml)

# Surreal

A simple hobby game engine built with modern .NET technologies.

## About The Project

Surreal is a simple game engine built with the most recent .NET tech and C# features.
It is designed mainly as a learning tool for game engine architecture and design.

It's similar in scope to existing game engines like [MonoGame](https://www.monogame.net/) and [libGDX](https://libgdx.com/),
with an emphasis on simplicity and ease of use.

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet)

### Installation

1. Clone the repository ```sh git clone https://github.com/mattkleiny/surreal.git```
2. Build the project ```sh dotnet build```
3. Run the tests ```sh dotnet test```
4. Run an example ```sh dotnet run --project ./examples/HelloWorld```

## Usage

The following code snippets show how to use Surreal to create a simple game.

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

## Capabilities

- [x] Windows support
- [ ] Mac support
- [ ] Linux support
- [ ] Android support
- [ ] iOS support
- [ ] WASM support

### Audio

- [ ] Audio playback
- [ ] Audio hot reloading
- [ ] Audio library
- [ ] Audio effects

### Graphics

- [x] Mesh creation and rendering
- [x] Texture read/writes
- [ ] Texture hot reloading
- [x] 2D Sprites and sprite sheets
- [ ] 3D Sprites and sprite sheets
- [x] 2D camera support
- [ ] 3D camera support
- [ ] Text rendering
- [x] Render targets
- [x] Post processing effects
- [x] Material system
- [ ] Material batching
- [ ] Compute shader support
- [ ] Shader hot reloading
- [ ] Shader reflection
- [x] Shader preprocessor
- [ ] Shader library

### Scripting

- [ ] Shared scripting VM
- [ ] Script hot reloading
- [ ] Script library
- [x] Lua support
