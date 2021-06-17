using System;
using System.Collections;
using System.Collections.Generic;
using Surreal.Collections;

namespace Surreal.Graphics.Materials.Shaders.Spirv {
  internal abstract class Instruction {
    public abstract void Compile(BytecodeBuffer buffer);
  }

  internal sealed class InstructionList : IReadOnlyList<Instruction> {
    private readonly List<Instruction> instructions = new();

    public int Count => instructions.Count;

    public Instruction this[int index] => instructions[index];
    public Instruction this[Index index] => instructions[index];

    public ListSpan<Instruction> this[Range range] => new(instructions, range);

    public void Add(Instruction instruction)    => instructions.Add(instruction);
    public void Remove(Instruction instruction) => instructions.Remove(instruction);
    public void Clear()                         => instructions.Clear();

    public void Compile(BytecodeBuffer buffer) {
      for (var i = 0; i < instructions.Count; i++) {
        instructions[i].Compile(buffer);
      }
    }

    public List<Instruction>.Enumerator               GetEnumerator() => instructions.GetEnumerator();
    IEnumerator<Instruction> IEnumerable<Instruction>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.                          GetEnumerator() => GetEnumerator();
  }
}