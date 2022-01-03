namespace Surreal;

/// <summary>A specialization of <see cref="IPlatformHost"/> for desktop environments.</summary>
public interface IDesktopPlatformHost : IPlatformHost
{
  IDesktopWindow Window { get; }
}
