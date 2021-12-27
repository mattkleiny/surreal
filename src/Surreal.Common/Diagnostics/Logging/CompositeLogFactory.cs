namespace Surreal.Diagnostics.Logging;

public sealed class CompositeLogFactory : ILogFactory
{
	private readonly ILogFactory[] factories;

	public CompositeLogFactory(params ILogFactory[] factories)
	{
		this.factories = factories;
	}

	public ILog GetLog(string category)
	{
		return new CompositeLog(factories.Select(factory => factory.GetLog(category)));
	}

	private sealed class CompositeLog : ILog
	{
		private readonly ILog[] logs;

		public CompositeLog(IEnumerable<ILog> logs)
		{
			this.logs = logs.ToArray();
		}

		public bool IsLevelEnabled(LogLevel level)
		{
			for (var i = 0; i < logs.Length; i++)
			{
				var log = logs[i];

				if (log.IsLevelEnabled(level)) return true;
			}

			return false;
		}

		public void WriteMessage(LogLevel level, string message)
		{
			for (var i = 0; i < logs.Length; i++)
			{
				var log = logs[i];

				if (log.IsLevelEnabled(level))
				{
					log.WriteMessage(level, message);
				}
			}
		}
	}
}
