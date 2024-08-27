namespace Surreal.Physics;

public class PhysicsHandleTests
{
  [Test]
  public unsafe void it_should_convert_pointer_to_and_from_handle()
  {
    var value = (void*)0x1234;
    var handle = PhysicsHandle.FromPointer(value);

    var result = handle.AsPointer();

    Assert.True(value == result);
  }
}
