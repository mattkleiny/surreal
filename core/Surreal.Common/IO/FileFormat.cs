using System.Xml.Serialization;

namespace Surreal.IO;

/// <summary>
/// A format for I/O file operations.
/// </summary>
public abstract class FileFormat
{
  public static FileFormat Json { get; } = new JsonFileFormat();
  public static FileFormat Xml { get; } = new XmlFileFormat();

  #region Serialization

  public abstract void Serialize(Stream stream, object value);
  public abstract void Serialize<T>(Stream stream, T value) where T : class;
  public abstract Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = default);
  public abstract Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default) where T : class;

  #endregion

  #region Deserialization

  public abstract object? Deserialize(Stream stream, Type type);
  public abstract T? Deserialize<T>(Stream stream) where T : class;
  public abstract Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default);
  public abstract Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default) where T : class;

  #endregion

  /// <summary>
  /// A <see cref="FileFormat"/> for JSON.
  /// </summary>
  private sealed class JsonFileFormat : FileFormat
  {
    public override void Serialize(Stream stream, object value)
    {
      JsonSerializer.Serialize(stream, value);
    }

    public override void Serialize<T>(Stream stream, T value)
    {
      JsonSerializer.Serialize(stream, value);
    }

    public override async Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = default)
    {
      await JsonSerializer.SerializeAsync(stream, value, cancellationToken: cancellationToken);
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

    public override T? Deserialize<T>(Stream stream) where T : class
    {
      return JsonSerializer.Deserialize<T>(stream);
    }

    public override async Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
      return await JsonSerializer.DeserializeAsync(stream, type, cancellationToken: cancellationToken);
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
      var serializer = new XmlSerializer(value.GetType());

      serializer.Serialize(stream, value);
    }

    public override void Serialize<T>(Stream stream, T value)
    {
      var serializer = new XmlSerializer(typeof(T));

      serializer.Serialize(stream, value);
    }

    public override Task SerializeAsync(Stream stream, object value, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var serializer = new XmlSerializer(value.GetType());

      serializer.Serialize(stream, value);

      return Task.CompletedTask;
    }

    public override Task SerializeAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
      cancellationToken.ThrowIfCancellationRequested();

      var serializer = new XmlSerializer(typeof(T));

      serializer.Serialize(stream, value);

      return Task.CompletedTask;
    }

    public override object? Deserialize(Stream stream, Type type)
    {
      var serializer = new XmlSerializer(type);

      return serializer.Deserialize(stream);
    }

    public override T? Deserialize<T>(Stream stream)
      where T : class
    {
      var serializer = new XmlSerializer(typeof(T));

      return (T?)serializer.Deserialize(stream);
    }

    public override Task<object?> DeserializeAsync(Stream stream, Type type, CancellationToken cancellationToken = default)
    {
      var serializer = new XmlSerializer(type);

      return Task.FromResult(serializer.Deserialize(stream));
    }

    public override Task<T?> DeserializeAsync<T>(Stream stream, CancellationToken cancellationToken = default) where T : class
    {
      var serializer = new XmlSerializer(typeof(T));

      return Task.FromResult((T?)serializer.Deserialize(stream));
    }
  }
}
