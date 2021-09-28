using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Surreal.Mechanics.Commands
{
  public class CommandQueue
  {
    private readonly Queue<ICommand> commands = new();

    public void Enqueue(ICommand command)
    {
      commands.Enqueue(command);
    }

    public bool TryDequeue([MaybeNullWhen(false)] out ICommand command)
    {
      return commands.TryDequeue(out command);
    }
  }
}
