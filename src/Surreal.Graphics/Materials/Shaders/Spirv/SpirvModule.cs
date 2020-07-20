using System;

namespace Surreal.Graphics.Materials.Shaders.Spirv {
  internal sealed class SpirvModule {
    public Capabilities    Capabilities    { get; set; } = Capabilities.None;
    public ExecutionMode   ExecutionMode   { get; set; } = ExecutionMode.None;
    public AddressingModel AddressingModel { get; set; } = AddressingModel.Logical;
    public InstructionList Instructions    { get; }      = new InstructionList();

    public Memory<byte> Compile() {
      var buffer = new BytecodeBuffer();

      Instructions.Compile(buffer);

      return buffer;
    }

    public static SpirvModule Decompile(Memory<byte> bytecode) {
      throw new NotImplementedException();
    }
  }
}