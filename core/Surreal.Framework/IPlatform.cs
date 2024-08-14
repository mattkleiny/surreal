namespace Surreal;

/// <summary>
/// Represents the underlying platform.
/// </summary>
public interface IPlatform
{
  /// <summary>
  /// Builds the main host for the platform.
  /// </summary>
  IPlatformHost BuildHost();
}
