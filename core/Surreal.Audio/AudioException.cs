namespace Surreal.Audio;

/// <summary>
/// Represents an error in the audio system.
/// </summary>
public class AudioException(string? message, Exception? innerException = null) : ApplicationException(message, innerException);
