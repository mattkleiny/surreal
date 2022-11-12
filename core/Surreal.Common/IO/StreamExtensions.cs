using System.Runtime.CompilerServices;

namespace Surreal.IO;

/// <summary>Extensions related to streaming and I/O.</summary>
public static class StreamExtensions
{
  /// <summary>Asynchronously reads all lines from the given <see cref="TextReader" />.</summary>
  public static async IAsyncEnumerable<string> ReadLinesAsync(this TextReader reader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    while (true)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var line = await reader.ReadLineAsync();
      if (line == null)
      {
        yield break;
      }

      yield return line;
    }
  }
}



