using NUnit.Framework;
using Surreal.Mathematics;

namespace Surreal.Serialization;

public class SerializationTests
{
  private static SerializationContext Context { get; } = new();

  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Angle value)       => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Area value)        => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(BoundingBox value) => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Color value)       => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(FloatRange value)  => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Guid value)        => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(IntRange value)    => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Quaternion value)  => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Rectangle value)   => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Seed value)        => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Vector2I value)    => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Vector2 value)     => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Vector3I value)    => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Vector3 value)     => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Vector4 value)     => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(VolumeI value)     => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
  [Test, AutoFixture] public ValueTask it_should_serialize_basic_types(Volume value)      => Context.SerializeAsync(value, new StreamSerializerWriter(new MemoryStream()));
}
