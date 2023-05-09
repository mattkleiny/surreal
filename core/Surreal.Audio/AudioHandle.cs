namespace Surreal.Audio;

/// <summary>
/// An opaque handle to a resource in the underling <see cref="IAudioBackend" /> implementation.
/// </summary>
public readonly record struct AudioHandle(nint Id)
{
  public static AudioHandle None => default;

  public static implicit operator nint(AudioHandle handle)
  {
    return handle.Id;
  }

  public static implicit operator int(AudioHandle handle)
  {
    return (int)handle.Id;
  }

  public static implicit operator uint(AudioHandle handle)
  {
    return (uint)handle.Id;
  }
}
