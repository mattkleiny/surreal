namespace Surreal.IO;

/// <summary>
/// Represents a type that can be serialized to and from a JSON string.
/// </summary>
public interface IJsonSerializable
{
  /// <summary>
  /// Converts the given JSON string to this type.
  /// </summary>
  static abstract object FromJson(string json);

  /// <summary>
  /// Converts this type to a JSON string.
  /// </summary>
  string ToJson();
}

/// <summary>
/// Represents a type that can be serialized to and from a JSON string.
/// </summary>
public interface IJsonSerializable<out TSelf> : IJsonSerializable
  where TSelf : IJsonSerializable<TSelf>
{
  /// <summary>
  /// Converts the given JSON string to this type.
  /// </summary>
  new static abstract TSelf FromJson(string json);

  /// <inheritdoc/>
  static object IJsonSerializable.FromJson(string json) => TSelf.FromJson(json);
}
