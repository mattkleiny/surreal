namespace Surreal;

[Ignore("Not implemented yet")]
public class VariantTests
{
  [Test]
  public void it_should_convert_bool()
  {
    Variant test = true;

    Assert.AreEqual(VariantType.Bool, test.Type);
    Assert.AreEqual(true, test.AsBool());
  }

  [Test]
  public void it_should_convert_byte()
  {
    Variant test = (byte)1;

    Assert.AreEqual(VariantType.Byte, test.Type);
    Assert.AreEqual((byte)1, test.AsByte());
  }

  [Test]
  public void it_should_convert_short()
  {
    Variant test = (short)1;

    Assert.AreEqual(VariantType.Short, test.Type);
    Assert.AreEqual((short)1, test.AsShort());
  }


  [Test]
  public void it_should_convert_int()
  {
    Variant test = 1;

    Assert.AreEqual(VariantType.Int, test.Type);
    Assert.AreEqual(1, test.AsInt());
  }


  [Test]
  public void it_should_convert_float()
  {
    Variant test = 1f;

    Assert.AreEqual(VariantType.Float, test.Type);
    Assert.AreEqual(1f, test.AsFloat());
  }

  [Test]
  public void it_should_convert_string()
  {
    Variant test = "test";

    Assert.AreEqual(VariantType.String, test.Type);
    Assert.AreEqual("test", test.AsString());
  }

  [Test]
  public void it_should_convert_object()
  {
    Variant test = Variant.From(new object());

    Assert.AreEqual(VariantType.Object, test.Type);
    Assert.AreEqual(typeof(object), test.AsObject()!.GetType());
  }

  [Test]
  public void it_should_convert_array()
  {
    Variant test = new int[32];

    Assert.AreEqual(VariantType.Array, test.Type);
    Assert.AreEqual(typeof(int[]), test.AsObject()!.GetType());
  }
}
