namespace Surreal.Framework.Screens {
  public interface ILoadNotifier {
    int   MaxCount { get; set; }
    float Progress { get; set; }

    void Increment(int amount = 1);
    void Increment(float amount = 0.1f);
  }
}