namespace Surreal.Audio.Synthesis;

/// <summary>A plan for audio synthesis in an <see cref="AudioSynthesizer"/>.</summary>
public interface IAudioSynthesisPlan
{
  Task EmitAsync(IAudioSynthesisPipe pipe, CancellationToken cancellationToken = default);
}
