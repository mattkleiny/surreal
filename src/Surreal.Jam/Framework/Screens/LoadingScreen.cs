using System.Diagnostics;
using System.Threading.Tasks;
using Surreal.Diagnostics.Logging;
using Surreal.Utilities;

namespace Surreal.Framework.Screens {
  public abstract class LoadingScreen<TGame, TScreen> : GameScreen<TGame>, ILoadNotifier
      where TGame : GameJam
      where TScreen : class, ILoadableScreen {
    private static readonly ILog Log = LogFactory.GetLog<LoadingScreen<TGame, TScreen>>();

    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly TScreen   screen;
    private          Task?     loadingTask;
    private          bool      finalized;

    protected LoadingScreen(TGame game, TScreen screen)
        : base(game) {
      this.screen = screen;
    }

    public int   MaxCount { get; set; }
    public float Progress { get; set; }

    void ILoadNotifier.Increment(int amount) {
      Progress += (float) amount / MaxCount;
    }

    void ILoadNotifier.Increment(float progress) {
      Progress += progress;
    }

    public override void Initialize() {
      base.Initialize();

      // kick off a task to load and initialize the target state
      stopwatch.Start();

      loadingTask = Task.Run(() => screen.LoadInBackgroundAsync(screen.Assets, this));
    }

    public override void Update(GameTime time) {
      base.Update(time);

      if (loadingTask?.IsCompleted == true && !finalized) {
        stopwatch.Stop();

        // waiting on the completed task will propagate any faults
        if (loadingTask.IsFaulted) {
          loadingTask.Wait();
        }

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
  }
}