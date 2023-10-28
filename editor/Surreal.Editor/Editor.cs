using Avalonia;
using Surreal.Diagnostics.Logging;
using Surreal.Diagnostics.Profiling;
using Surreal.Editing;
using Surreal.Editing.Projects;
using Surreal.Utilities;

namespace Surreal;

/// <summary>
/// Configuration for the editor application.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record EditorConfiguration
{
  /// <summary>
  /// The default window title for the editor.
  /// </summary>
  public string WindowTitle { get; init; } = "Surreal";

  /// <summary>
  /// The default width of the editor window.
  /// </summary>
  public int DefaultWidth { get; init; } = 1280;

  /// <summary>
  /// The default height of the editor window.
  /// </summary>
  public int DefaultHeight { get; init; } = 720;

  /// <summary>
  /// The default <see cref="EditorProject"/> to open when starting the editor.
  /// </summary>
  public EditorProject? DefaultProject { get; init; }

  /// <summary>
  /// The <see cref="IServiceModule"/>s to use for the editor.
  /// </summary>
  public List<IServiceModule> Modules { get; init; } = new();
}

/// <summary>
/// Entry point for the editor.
/// </summary>
[ExcludeFromCodeCoverage]
public class Editor
{
  private static readonly ILog Log = LogFactory.GetLog<Editor>();

  /// <summary>
  /// Sets up the logging and profiling systems.
  /// </summary>
  [ModuleInitializer]
  [SuppressMessage("Usage", "CA2255:The \'ModuleInitializer\' attribute should not be used in libraries")]
  internal static void SetupLogging()
  {
    LogFactory.Current = new TextWriterLogFactory(Console.Out, LogLevel.Trace, LogFormatters.Default());
    ProfilerFactory.Current = new SamplingProfilerFactory(new InMemoryProfilerSampler());
  }

  /// <summary>
  /// Starts the editor.
  /// </summary>
  public static int Start(EditorConfiguration configuration)
  {
    var builder = AppBuilder
      .Configure(() => new EditorApplication(configuration))
      .UsePlatformDetect()
      .LogToTrace();

    Log.Trace("Starting editor");

    return builder.StartWithClassicDesktopLifetime(Environment.GetCommandLineArgs());
  }
}
