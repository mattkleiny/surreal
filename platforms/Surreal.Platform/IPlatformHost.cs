using Surreal.Threading;
using Surreal.Timing;

namespace Surreal;

/// <summary>Represents the underlying platform host for the engine.</summary>
public interface IPlatformHost : IDisposable
{
  event Action<int, int> Resized;

  int  Width     { get; }
  int  Height    { get; }
  bool IsVisible { get; }
  bool IsFocused { get; }
  bool IsClosing { get; }

  IServiceModule Services   { get; }
  IDispatcher    Dispatcher { get; }

  void Tick(DeltaTime deltaTime);
}
