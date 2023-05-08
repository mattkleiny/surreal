using System.Runtime.CompilerServices;

namespace Surreal.IO;

/// <summary>
/// Extensions related to <see cref="TextReader"/>.
/// </summary>
public static class TextReaderExtensions
{
  /// <summary>
  /// Asynchronously reads all lines from the given <see cref="TextReader" />.
  /// </summary>
  public static async IAsyncEnumerable<string> ReadLinesAsync(this TextReader reader, [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    while (true)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var line = await reader.ReadLineAsync(cancellationToken);

      if (line == null)
      {
        yield break;
      }

      yield return line;
    }
  }
}
