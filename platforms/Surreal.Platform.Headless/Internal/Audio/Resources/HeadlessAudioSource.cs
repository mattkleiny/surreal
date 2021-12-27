using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Mathematics;

namespace Surreal.Internal.Audio.Resources;

internal sealed class HeadlessAudioSource : AudioSource
{
	private float volume;

	public override float Volume
	{
		get => volume;
		set => volume = value.Clamp(0f, 1f);
	}

	public override bool IsPlaying => false;

	public override void Play(AudioClip clip)
	{
	}
}
