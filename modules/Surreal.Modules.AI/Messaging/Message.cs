using System.Runtime.CompilerServices;

namespace Surreal.Messaging;

/// <summary>A message that carries a stack-allocated message to recipients</summary>
public readonly unsafe ref struct Message
{
  private readonly void* pointer;
  private readonly Type  type;

  public static Message Create<T>(ref T payload)
  {
    var pointer = Unsafe.AsPointer(ref payload);
    var type    = typeof(T);

    return new Message(pointer, type);
  }

  private Message(void* pointer, Type type)
  {
    this.pointer = pointer;
    this.type    = type;
  }

  public bool Is<T>()
  {
    return type == typeof(T);
  }

  public ref T Cast<T>()
  {
    return ref Unsafe.AsRef<T>(pointer);
  }
}