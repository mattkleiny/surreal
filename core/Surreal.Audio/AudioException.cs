namespace Surreal.Audio;

/// <summary>Represents an error in the audio system.</summary>
public class AudioException : Exception
{
  public AudioException(string? message, Exception? innerException = null)
    : base(message, innerException)
  {
  }
}
