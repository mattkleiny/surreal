using Surreal.IO.Binary;
using Surreal.Mathematics;

namespace Surreal.IO;

public class BinarySerializationTests
{
  [Test, AutoFixture] public ValueTask it_should_serialize_Angle(Angle value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Area(Area value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_BoundingBox(BoundingBox value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Color(Color value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_FloatRange(FloatRange value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Guid(Guid value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_IntRange(IntRange value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Quaternion(Quaternion value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Rectangle(Rectangle value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Seed(Seed value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Vector2I(Vector2I value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Vector2(Vector2 value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Vector3I(Vector3I value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Vector3(Vector3 value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Vector4(Vector4 value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_VolumeI(VolumeI value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_Volume(Volume value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_TimeSpan(TimeSpan value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  [Test, AutoFixture] public ValueTask it_should_serialize_TimeSpanRange(TimeSpanRange value)
    => BinarySerializer.SerializeAsync(value, CreateWriter());

  private static StreamBinaryWriter CreateWriter() => new(new MemoryStream());
}
