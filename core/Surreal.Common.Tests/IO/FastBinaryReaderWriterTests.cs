namespace Surreal.IO;

public class FastBinaryReaderWriterTests
{
  [Test]
  public void it_should_serialize_to_binary()
  {
    var output = new MemoryStream();
    var writer = new FastBinaryWriter(output);

    writer.WriteInt16(42);
    writer.WriteInt32(42);
    writer.WriteInt64(42);
    writer.WriteFloat(42.0f);
    writer.WriteDouble(42.0);
    writer.WriteVector2(Vector2.UnitY);
    writer.WriteString("John Doe");

    output.Position = 0;

    var reader = new FastBinaryReader(output);

    reader.ReadInt16().Should().Be(42);
    reader.ReadInt32().Should().Be(42);
    reader.ReadInt64().Should().Be(42);
    reader.ReadFloat().Should().Be(42.0f);
    reader.ReadDouble().Should().Be(42.0);
    reader.ReadVector2().Should().Be(Vector2.UnitY);
    reader.ReadString().Should().Be("John Doe");
  }
}
