using NUnit.Framework;

namespace Surreal.Collections;

public class SlotMapTests
{
  [Test]
  public void it_should_return_a_valid_key()
  {
    var map = new SlotMap<object>();
    var obj = new object();

    var key = map.Add(obj);
    Assert.AreEqual(obj, map.Get(key));

    map.TryGet(key, out var result);
    Assert.AreEqual(obj, result);
  }

  [Test]
  public void it_should_return_for_an_invalid_key()
  {
    var map = new SlotMap<object>();
    var ok  = map.TryGet(0, out var result);

    Assert.IsFalse(ok);
    Assert.IsNull(result);
  }

  [Test]
  public void it_should_remove_a_valid_key()
  {
    var map = new SlotMap<object>();
    var obj = new object();

    var key = map.Add(obj);
    map.Remove(key);
    var ok = map.TryGet(key, out var result);

    Assert.IsFalse(ok);
    Assert.IsNull(result);
  }

  [Test]
  public void it_should_not_remove_invalid_keys()
  {
    var map = new SlotMap<object>();
    var ok  = map.TryRemove(0);

    Assert.IsFalse(ok);
  }

  [Test]
  public void it_should_reuse_keys_in_stable_manner()
  {
    var map   = new SlotMap<object>();
    var value = new object();

    var key1 = map.Add(value);
    map.Remove(key1);
    var key2 = map.Add(value);

    var ok = map.TryGet(key1, out object result);

    Assert.IsFalse(ok);
    Assert.IsNull(result);

    ok = map.TryGet(key2, out result);

    Assert.IsTrue(ok);
    Assert.AreEqual(value, result);
  }

  [Test]
  public void it_should_throw_exception_if_key_does_not_exist()
  {
    var map = new SlotMap<object>();

    Assert.Throws<ObjectNotFoundException>(() => map.Get(0));
    Assert.Throws<ObjectNotFoundException>(() => map.Remove(0));
  }
}
