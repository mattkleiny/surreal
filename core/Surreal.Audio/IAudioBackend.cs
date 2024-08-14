namespace Surreal.Audio;

/// <summary>
/// Represents a device capable of playing audio.
/// </summary>
public interface IAudioBackend
{
  static IAudioBackend Null { get; } = new NullAudioBackend();

  /// <summary>
  /// Creates a new <see cref="IAudioDevice"/>.
  /// </summary>
  IAudioDevice CreateDevice();

  /// <summary>
  /// A no-op <see cref="IAudioBackend"/> for headless environments and testing.
  /// </summary>
  [ExcludeFromCodeCoverage]
  private sealed class NullAudioBackend : IAudioBackend
  {
    public IAudioDevice CreateDevice()
    {
      return IAudioDevice.Null;
    }
  }
}
