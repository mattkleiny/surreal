using Surreal.Audio;
using Surreal.Utilities;

namespace Surreal.Scenes.Spatial.Audio;

/// <summary>
/// A node that listens to audio.
/// </summary>
public class AudioListener2D : SceneNode2D
{
  private float _volume = 1f;

  /// <summary>
  /// The volume of the audio listener.
  /// </summary>
  public float Volume
  {
    get => _volume;
    set
    {
      SetField(ref _volume, value);

      if (IsInTree && Services.TryGetService(out IAudioBackend backend))
      {
        backend.SetAudioListenerGain(_volume);
      }
    }
  }

  protected override void OnEnterTree()
  {
    base.OnEnterTree();

    if (Services.TryGetService(out IAudioBackend backend))
    {
      backend.SetAudioListenerGain(_volume);
      backend.SetAudioListenerPosition(new Vector3(GlobalPosition, 0f));
    }
  }

  protected override void OnTransformUpdated()
  {
    base.OnTransformUpdated();

    if (Services.TryGetService(out IAudioBackend backend))
    {
      backend.SetAudioListenerPosition(new Vector3(GlobalPosition, 0f));
    }
  }
}
