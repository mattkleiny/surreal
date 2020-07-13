using System.Diagnostics;
using System.Threading.Tasks;
using Surreal.Assets;
using Surreal.Diagnostics.Logging;
using Surreal.IO;

namespace Surreal.Framework.Screens {
  public abstract class LoadingScreen<TGame, TScreen> : GameScreen<TGame>
      where TGame : GameJam
      where TScreen : class, ILoadableScreen {
    private static readonly ILog Log = LogFactory.GetLog<LoadingScreen<TGame, TScreen>>();

    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly TScreen   screen;

    private TrackingAssetResolver? resolver;
    private Task?                  loadingTask;
    private bool                   finalized;

    protected LoadingScreen(TGame game, TScreen screen)
        : base(game) {
      this.screen = screen;
    }

    public int   ExpectedAssets => resolver?.ExpectedAssets ?? 1;
    public int   LoadedAssets   => resolver?.LoadedAssets   ?? 1;
    public float Progress       => (float) LoadedAssets / ExpectedAssets;

    public override void Initialize() {
      base.Initialize();

      // kick off a task to load and initialize the target state
      stopwatch.Start();

      resolver    = new TrackingAssetResolver(screen.Assets);
      loadingTask = Task.Run(() => screen.LoadInBackgroundAsync(resolver));
    }

    public override void Update(GameTime time) {
      base.Update(time);

      if (loadingTask?.IsCompleted == true && !finalized) {
        stopwatch.Stop();

        // waiting on the completed task will propagate any faults
        if (loadingTask.IsFaulted) loadingTask.Wait();

        // swap to the new state at the start of the next frame
        Engine.Schedule(() => {
          Log.Trace($"{screen.GetType().GetFullNameWithoutGenerics()} loaded successfully. Time taken: {stopwatch.Elapsed:g}");

          Game.Screens.Pop();
          Game.Screens.Push(screen);
        });

        // we need to add an extra flag in case of a fixed step/multiple update strategy
        finalized = true;
      }
    }

    public override void Dispose() {
      if (!loadingTask?.IsCompleted ?? false) {
        screen.Dispose();
      }

      base.Dispose();
    }

    private sealed class TrackingAssetResolver : IAssetResolver, IAssetManifest {
      private readonly IAssetResolver resolver;

      public TrackingAssetResolver(IAssetResolver resolver) {
        this.resolver = resolver;
      }

      public int ExpectedAssets { get; private set; }
      public int LoadedAssets   { get; private set; }

      public void Add<TAsset>(Path path) {
        ExpectedAssets++;
      }

      public Task<TAsset> GetAsync<TAsset>(Path path) {
        var asset = resolver.GetAsync<TAsset>(path);

        LoadedAssets++;

        return asset;
      }
    }
  }
}