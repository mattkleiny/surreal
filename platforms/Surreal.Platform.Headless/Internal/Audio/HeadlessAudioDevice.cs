using Surreal.Audio;
using Surreal.Audio.Clips;
using Surreal.Audio.Playback;
using Surreal.Internal.Audio.Resources;
using Surreal.Mathematics;

namespace Surreal.Internal.Audio;

internal sealed class HeadlessAudioDevice : IAudioDevice
{
	private float masterVolume;

	public float MasterVolume
	{
		get => masterVolume;
		set => masterVolume = value.Clamp(0f, 1f);
	}

	public AudioClip CreateAudioClip(IAudioData data)
	{
		return new HeadlessAudioClip();
	}

	public AudioSource CreateAudioSource()
	{
		return new HeadlessAudioSource();
	}
}
