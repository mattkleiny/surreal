using Surreal.IO;
using Surreal.Utilities;

namespace Surreal.Build.Targets;

/// <summary>
/// Configuration for the final build package.
/// </summary>
public sealed record PackageConfiguration
{
  public required string Name { get; init; }
  public required string OutputDirectory { get; init; }
  public VirtualPath Icon { get; set; }
}

/// <summary>
/// Context for the packaging steps.
/// </summary>
public sealed class PackageContext
{
  public required PackageConfiguration Configuration { get; init; }
  public required IProgressReporter ProgressReporter { get; init; }
  public required CancellationToken CancellationToken { get; init; }
}

/// <summary>
/// Possible target platforms for the packaging operations.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum PlatformIdentifier
{
  Windows,
  Mac,
  Linux,
  Android,
  iOS,
  Web,
  Switch,
}

/// <summary>
/// A target for building packages for various platforms.
/// </summary>
public abstract class PlatformTarget
{
  public static PlatformTarget Create(PlatformIdentifier identifier) => identifier switch
  {
    PlatformIdentifier.Windows => new WindowsTarget(),
    // PlatformIdentifier.Mac => throw new NotImplementedException(),
    // PlatformIdentifier.Linux => throw new NotImplementedException(),
    // PlatformIdentifier.Android => throw new NotImplementedException(),
    // PlatformIdentifier.iOS => throw new NotImplementedException(),
    // PlatformIdentifier.Web => throw new NotImplementedException(),
    // PlatformIdentifier.Switch => throw new NotImplementedException(),

    _ => throw new ArgumentOutOfRangeException(nameof(identifier), identifier, null)
  };

  /// <summary>
  /// Builds a package for the target platform using the given configuration.
  /// </summary>
  public abstract Task BuildPackageAsync(
    PackageConfiguration configuration,
    IProgressReporter reporter,
    CancellationToken cancellationToken = default);
}

/// <summary>
/// A <see cref="PlatformTarget"/> for Windows.
/// </summary>
public sealed class WindowsTarget : PlatformTarget
{
  public override async Task BuildPackageAsync(
    PackageConfiguration configuration,
    IProgressReporter reporter,
    CancellationToken cancellationToken = default)
  {
    using var operation = reporter.StartOperation("Building Windows package");

    var iconPath = VirtualPath.Combine(configuration.OutputDirectory, "icon.ico");

    await configuration.Icon.CopyToAsync(iconPath, cancellationToken);
  }
}

// public class MacTarget : IPlatformTarget;
// public class LinuxTarget : IPlatformTarget;
// public class AndroidTarget : IPlatformTarget;
// public class IosTarget : IPlatformTarget;
// public class WebTarget : IPlatformTarget;
// public class SwitchTarget : IPlatformTarget;
// public class Ps4Target : IPlatformTarget;
