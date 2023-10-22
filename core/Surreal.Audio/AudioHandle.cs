namespace Surreal.Audio;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IAudioBackend" /> implementation.
/// </summary>
[ExcludeFromCodeCoverage]
public readonly record struct AudioHandle(nint Id)
{
  public static AudioHandle None => default;
  public static AudioHandle Invalid => new(-1);

  public AudioHandle(uint Id)
    : this((nint)Id)
  {
  }

  public AudioHandle(int Id)
    : this((nint)Id)
  {
  }

  public static implicit operator nint(AudioHandle handle) => handle.Id;
  public static implicit operator int(AudioHandle handle) => (int)handle.Id;
  public static implicit operator uint(AudioHandle handle) => (uint)handle.Id;
}
