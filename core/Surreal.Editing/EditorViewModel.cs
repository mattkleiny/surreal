using Surreal.Utilities;
using Surreal.Workloads;

namespace Surreal;

/// <summary>Top-level <see cref="ViewModel"/> for the entire editor.</summary>
public sealed record EditorViewModel : ViewModel
{
  public GameViewModel? GameView => Editor.Services.GetService<GameViewModel>();
}
