using Surreal.Audio.Clips;

namespace Surreal.Platform.Internal.Audio.Resources {
  internal sealed class HeadlessAudioClip : AudioClip {
    protected override void Upload(IAudioData? existingData, IAudioData newData) {
      // no-op
    }
  }
}