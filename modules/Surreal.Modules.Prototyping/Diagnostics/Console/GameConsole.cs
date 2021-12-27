using System.Diagnostics;
using Surreal.Collections;

namespace Surreal.Diagnostics.Console;

/// <summary>A console for developer input.</summary>
public interface IGameConsole
{
	IEnumerable<string> History { get; }

	void WriteLine(string element);
	void Evaluate(string expression);
	void Clear();
}

/// <summary>The default <see cref="IGameConsole"/>.</summary>
public sealed class GameConsole : IGameConsole
{
	private readonly IGameConsoleInterpreter interpreter;
	private readonly RingBuffer<string> history;

	public GameConsole(IGameConsoleInterpreter interpreter, int capacity = 1000)
	{
		Debug.Assert(capacity > 0, "capacity > 0");

		this.interpreter = interpreter;

		history = new RingBuffer<string>(capacity);
	}

	public IEnumerable<string> History => history;

	public void WriteLine(string element)
	{
		history.Add(element);
	}

	public void Evaluate(string expression)
	{
		WriteLine(interpreter.Evaluate(expression));
	}

	public void Clear()
	{
		history.Clear();
	}
}
