using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using OpenTK.Audio.OpenAL;
using Surreal.Audio.Clips;

namespace Surreal.Platform.Internal.Audio.Resources {
  [DebuggerDisplay("Audio Clip {Duration} ~{Size}")]
  internal sealed class OpenTKAudioClip : AudioClip {
    public readonly int Id = AL.GenBuffer();

    public OpenTKAudioClip(IAudioData data) => Upload(data);

    protected override unsafe void Upload(IAudioData? existingData, IAudioData newData) {
      var (frequency, channels, bitsPerSample) = newData.Rate;

      var raw     = newData.Span;
      var pointer = Unsafe.AsPointer(ref raw.GetPinnableReference());
      var format  = GetSoundFormat(channels, bitsPerSample);

      AL.BufferData(Id, format, new IntPtr(pointer), raw.Length, frequency);
    }

    protected override void Dispose(bool managed) {
      AL.DeleteBuffer(Id);

      base.Dispose(managed);
    }

    private static ALFormat GetSoundFormat(int channels, int bits) => channels switch {
        1 => bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16,
        2 => bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16,
        _ => throw new NotSupportedException("The specified sound format is not supported.")
    };
  }
}