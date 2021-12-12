namespace Surreal;

/// <summary>Represents the underlying platform.</summary>
public interface IPlatform
{
  IPlatformHost BuildHost();
}
