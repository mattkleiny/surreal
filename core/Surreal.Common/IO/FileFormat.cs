namespace Surreal;

/// <summary>
/// A format for I/O file operations.
/// </summary>
public abstract class FileFormat
{
  public static FileFormat Json { get; } = new JsonFileFormat();
  public static FileFormat Xml { get; } = new XmlFileFormat();

  /// <summary>
  /// Serializes the given value to the given stream.
  /// </summary>
  public abstract void Serialize(Stream stream, object value);

  /// <summary>
  /// Serializes the given value to the given stream.
  /// </summary>
  public abstract Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = default);

  /// <summary>
  /// Serializes the given value to the given stream.
  /// </summary>
  public abstract void Serialize<T>(Stream stream, T value) where T : class;

  /// <summary>
  /// Serializes the given value to the given stream.
  /// </summary>
  public abstract Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default) where T : class;

  /// <summary>
  /// Deserializes a value of the given type from the given stream.
  /// </summary>
  public abstract object? Deserialize(Stream stream, Type type);

  /// <summary>
  /// Deserializes a value of the given type from the given stream.
  /// </summary>
  public abstract Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);

  /// <summary>
  /// Deserializes a value of the given type from the given stream.
  /// </summary>
  public abstract T? Deserialize<T>(Stream stream) where T : class;

  /// <summary>
  /// Deserializes a value of the given type from the given stream.
  /// </summary>
  public abstract Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default) where T : class;

  /// <summary>
  /// A <see cref="FileFormat"/> for JSON.
  /// </summary>
  private sealed class JsonFileFormat : FileFormat
  {
    public override void Serialize(Stream stream, object value)
    {
      JsonSerializer.Serialize(stream, value);
    }

    public override async Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = default)
    {
      await JsonSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken);
    }

    public override void Serialize<T>(Stream stream, T value)
    {
      JsonSerializer.Serialize(stream, value);
    }

    public override async Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
      where T : class
    {
      await JsonSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken);
    }

    public override object? Deserialize(Stream stream, Type type)
    {
      return JsonSerializer.Deserialize(stream, type);
    }

    public override async Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
      return await JsonSerializer.DeserializeAsync(stream, type, cancellationToken: cancellationToken);
    }

    public override T? Deserialize<T>(Stream stream) where T : class
    {
      return JsonSerializer.Deserialize<T>(stream);
    }

    public override async Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default)
      where T : class
    {
      return await JsonSerializer.DeserializeAsync<T>(stream, cancellationToken: cancellationToken);
    }
  }


  /// <summary>
  /// A <see cref="FileFormat"/> for XML.
  /// </summary>
  private sealed class XmlFileFormat : FileFormat
  {
    public override void Serialize(Stream stream, object value)
    {
      throw new NotImplementedException();
    }

    public override Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public override void Serialize<T>(Stream stream, T value)
    {
      throw new NotImplementedException();
    }

    public override Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public override object? Deserialize(Stream stream, Type type)
    {
      throw new NotImplementedException();
    }

    public override Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
      throw new NotImplementedException();
    }

    public override T? Deserialize<T>(Stream stream) where T : class
    {
      throw new NotImplementedException();
    }

    public override Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default) where T : class
    {
      throw new NotImplementedException();
    }
  }
}
