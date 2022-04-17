using Surreal.Areas;
using Surreal.Utilities;

namespace Surreal;

/// <summary>Top-level <see cref="ViewModel"/> for the entire editor.</summary>
public sealed record EditorViewModel : ViewModel
{
  public PlayModeViewModel PlayMode { get; } = new();
}
