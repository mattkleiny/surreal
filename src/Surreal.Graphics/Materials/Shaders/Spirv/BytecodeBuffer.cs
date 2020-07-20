using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Collections;

namespace Surreal.Graphics.Materials.Shaders.Spirv {
  internal sealed class BytecodeBuffer : IReadOnlyList<byte> {
    private readonly Bag<byte> bytecode = new Bag<byte>();

    public int Count => bytecode.Count;

    public byte this[int index] => bytecode[index];
    public byte this[Index index] => bytecode[index];
    public Memory<byte> this[Range range] => bytecode.Memory[range];

    public void Add(byte instruction) => bytecode.Add(instruction);
    public void Clear()               => bytecode.Clear();

    public Bag<byte>.Enumerator         GetEnumerator() => bytecode.GetEnumerator();
    IEnumerator<byte> IEnumerable<byte>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.            GetEnumerator() => GetEnumerator();

    public static implicit operator Memory<byte>(BytecodeBuffer buffer) => buffer[..];
  }
}S