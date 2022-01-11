namespace Surreal.Scripting.Bytecode;

public class BytecodeVirtualMachineTests
{
  [Test]
  public async Task it_should_execute_a_simple_program()
  {
    var machine = new BytecodeVirtualMachine
    {
      IntrinsicFunctions =
      {
        new("Draw", () => 1),
        new("Print", (string text) => Console.WriteLine(text)),
      }
    };

    var program = new BytecodeProgram
    {
      Instructions = ImmutableList.Create(
        new BytecodeInstruction(InstructionType.Nop),
        new BytecodeInstruction(InstructionType.Yield)
      ),
    };

    await machine.ExecuteAsync(program);
  }
}
