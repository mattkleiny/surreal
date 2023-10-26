using Surreal.Colors;
using Surreal.Maths;
using Surreal.Utilities;

namespace Surreal;

public class VariantTests
{
  [Test]
  public void it_should_be_32_bytes()
  {
    Unsafe.SizeOf<Variant>().Should().Be(32);
  }

  [Test]
  public void it_should_convert_bool()
  {
    Variant test = true;

    test.Type.Should().Be(VariantType.Bool);
    test.AsBool().Should().Be(true);
  }

  [Test]
  public void it_should_convert_byte()
  {
    Variant test = (byte)1;

    test.Type.Should().Be(VariantType.Byte);
    test.AsByte().Should().Be(1);
  }

  [Test]
  public void it_should_convert_short()
  {
    Variant test = (short)1;

    test.Type.Should().Be(VariantType.Short);
    test.AsShort().Should().Be(1);
  }

  [Test]
  public void it_should_convert_ushort()
  {
    Variant test = (ushort)1;

    test.Type.Should().Be(VariantType.Ushort);
    test.AsUshort().Should().Be(1);
  }

  [Test]
  public void it_should_convert_int()
  {
    Variant test = 1;

    test.Type.Should().Be(VariantType.Int);
    test.AsInt().Should().Be(1);
  }

  [Test]
  public void it_should_convert_uint()
  {
    Variant test = 1u;

    test.Type.Should().Be(VariantType.Uint);
    test.AsUint().Should().Be(1);
  }

  [Test]
  public void it_should_convert_long()
  {
    Variant test = 1L;

    test.Type.Should().Be(VariantType.Long);
    test.AsLong().Should().Be(1L);
  }

  [Test]
  public void it_should_convert_ulong()
  {
    Variant test = 1UL;

    test.Type.Should().Be(VariantType.Ulong);
    test.AsUlong().Should().Be(1L);
  }

  [Test]
  public void it_should_convert_float()
  {
    Variant test = 1f;

    test.Type.Should().Be(VariantType.Float);
    test.AsFloat().Should().Be(1f);
  }

  [Test]
  public void it_should_convert_double()
  {
    Variant test = 1d;

    test.Type.Should().Be(VariantType.Double);
    test.AsDouble().Should().Be(1d);
  }

  [Test]
  public void it_should_convert_decimal()
  {
    Variant test = 1m;

    test.Type.Should().Be(VariantType.Decimal);
    test.AsDecimal().Should().Be(1m);
  }

  [Test]
  public void it_should_convert_point2()
  {
    Variant test = Point2.One;

    test.Type.Should().Be(VariantType.Point2);
    test.AsPoint2().Should().Be(Point2.One);
  }

  [Test]
  public void it_should_convert_point3()
  {
    Variant test = Point3.One;

    test.Type.Should().Be(VariantType.Point3);
    test.AsPoint3().Should().Be(Point3.One);
  }

  [Test]
  public void it_should_convert_point4()
  {
    Variant test = Point4.One;

    test.Type.Should().Be(VariantType.Point4);
    test.AsPoint4().Should().Be(Point4.One);
  }

  [Test]
  public void it_should_convert_vector2()
  {
    Variant test = Vector2.One;

    test.Type.Should().Be(VariantType.Vector2);
    test.AsVector2().Should().Be(Vector2.One);
  }

  [Test]
  public void it_should_convert_vector3()
  {
    Variant test = Vector3.One;

    test.Type.Should().Be(VariantType.Vector3);
    test.AsVector3().Should().Be(Vector3.One);
  }

  [Test]
  public void it_should_convert_vector4()
  {
    Variant test = Vector4.One;

    test.Type.Should().Be(VariantType.Vector4);
    test.AsVector4().Should().Be(Vector4.One);
  }

  [Test]
  public void it_should_convert_quaternion()
  {
    Variant test = Quaternion.Identity;

    test.Type.Should().Be(VariantType.Quaternion);
    test.AsQuaternion().Should().Be(Quaternion.Identity);
  }

  [Test]
  public void it_should_convert_color()
  {
    Variant test = Color.White;

    test.Type.Should().Be(VariantType.Color);
    test.AsColor().Should().Be(Color.White);
  }

  [Test]
  public void it_should_convert_color32()
  {
    Variant test = Color32.White;

    test.Type.Should().Be(VariantType.Color32);
    test.AsColor32().Should().Be(Color32.White);
  }

  [Test]
  public void it_should_convert_string()
  {
    Variant test = "test";

    test.Type.Should().Be(VariantType.String);
    test.AsString().Should().Be("test");
  }

  [Test]
  public void it_should_convert_object()
  {
    var value = new object();
    Variant test = Variant.From(value);

    test.Type.Should().Be(VariantType.Object);
    test.AsObject().Should().Be(value);
  }

  [Test]
  public void it_should_convert_array()
  {
    var value = new int[32];
    Variant test = value;

    test.Type.Should().Be(VariantType.Array);
    test.AsArray().Should().Be(value);
  }

  [Test]
  public void it_should_convert_instance()
  {
    var value = new ServiceRegistry();
    Variant test = Variant.From(value);

    test.Type.Should().Be(VariantType.Object);
    test.AsObject().Should().Be(value);
    test.As<ServiceRegistry>().Should().Be(value);
  }

  [Test]
  public void it_should_convert_matrix4x4()
  {
    Variant test = Variant.From(Matrix4x4.Identity);

    test.Type.Should().Be(VariantType.Object);
    test.AsObject().Should().Be(Matrix4x4.Identity);
  }
}
