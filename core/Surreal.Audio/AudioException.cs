namespace Surreal.Audio;

/// <summary>Represents an error in the audio system.</summary>
public class AudioException : SurrealException
{
  public AudioException(string? message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
