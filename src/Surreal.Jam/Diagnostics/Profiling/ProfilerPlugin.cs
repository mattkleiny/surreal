using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Diagnostics.Profiling.Controls;
using Surreal.Framework;
using Surreal.Input.Keyboard;

namespace Surreal.Diagnostics.Profiling {
  public sealed class ProfilerPlugin : DiagnosticPlugin<GameJam> {
    private readonly InMemoryProfilerSampler sampler = new InMemoryProfilerSampler();

    public ProfilerPlugin(GameJam game)
        : base(game) {
    }

    public IKeyboardDevice Keyboard => Game.Keyboard;

    public override void Initialize() {
      ProfilerFactory.Current = new SamplingProfilerFactory(sampler);

      base.Initialize();
    }

    public override async Task LoadContentAsync(IAssetResolver assets) {
      await base.LoadContentAsync(assets);

      var font = await assets.LoadDefaultFontAsync();

      Stage.Add(new ProfilerPanel(Game.GraphicsDevice, font, sampler));
    }

    public override void Input(GameTime time) {
      if (Keyboard.IsKeyPressed(Key.F9)) IsVisible = !IsVisible;
      if (Keyboard.IsKeyPressed(Key.F8)) Task.Run(() => sampler.ExportToCSVAsync("./profiler.csv"));

      base.Input(time);
    }
  }
}