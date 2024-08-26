using Avalonia.Controls;
using Surreal.Editing.Workloads;
using Surreal.IO;

namespace HelloEditor.Workloads;

/// <summary>
/// An example workload for the editor.
/// </summary>
public sealed class TestWorkload : IEditorWorkload
{
  public void Configure(IEditorWorkloadConfiguration config)
  {
    config.RegisterDocumentType<TestLevel>();
    config.RegisterDocumentEditor<TestLevelEditor, TestLevel>();
    config.RegisterMenuCommand<CreateTestLevelCommand>("_New/_Test Level");
  }
}

/// <summary>
/// An editor command to create a new test level document.
/// </summary>
public sealed class CreateTestLevelCommand : IEditorCommand
{
  public void Execute(IEditorCommandContext context)
  {
    context.CreateAndOpenDocument<TestLevel>();
  }
}

/// <summary>
/// A test level document.
/// </summary>
public sealed class TestLevel : IEditorDocument
{
  public static IEditorDocumentFormat Format { get; } = new TestLevelFormat();

  public event Action? DocumentChanged;

  /// <summary>
  /// The format for saving and loading a test level document.
  /// </summary>
  private sealed class TestLevelFormat : IEditorDocumentFormat
  {
    public Task SaveAsync(VirtualPath file, object document)
    {
      throw new NotImplementedException();
    }

    public Task<object> LoadAsync(VirtualPath file)
    {
      throw new NotImplementedException();
    }
  }
}

/// <summary>
/// An editor control for a test level document.
/// </summary>
public sealed class TestLevelEditor : TextBlock, IEditorControl<TestLevel>;
