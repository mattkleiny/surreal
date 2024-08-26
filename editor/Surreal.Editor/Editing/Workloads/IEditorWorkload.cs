using Avalonia.Controls;
using Surreal.IO;

namespace Surreal.Editing.Workloads;

/// <summary>
/// A workload for the editor.
/// </summary>
public interface IEditorWorkload
{
  void Configure(IEditorWorkloadConfiguration config);
}

/// <summary>
/// Represents a document that can be edited in the editor.
/// </summary>
public interface IEditorDocument
{
  /// <summary>
  /// The format for saving and loading the document.
  /// </summary>
  static abstract IEditorDocumentFormat Format { get; }

  /// <summary>
  /// Raised when the document has changed.
  /// </summary>
  event Action DocumentChanged;
}

/// <summary>
/// A format for saving and loading an <see cref="IEditorDocument"/>.
/// </summary>
public interface IEditorDocumentFormat
{
  Task SaveAsync(VirtualPath file, object document);
  Task<object> LoadAsync(VirtualPath file);
}

/// <summary>
/// Represents a control for editing a <see cref="IEditorDocument"/>.
/// </summary>
public interface IEditorControl<[UsedImplicitly] TDocument>
  where TDocument : IEditorDocument;

/// <summary>
/// Represents a command that can be executed in the editor.
/// </summary>
public interface IEditorCommand
{
  void Execute(IEditorCommandContext context);
}

/// <summary>
/// A context for executing commands in the editor.
/// </summary>
public interface IEditorCommandContext
{
  void CreateAndOpenDocument<TDocument>()
    where TDocument : IEditorDocument;

  void CreateAndOpenDocument<TDocument>(TDocument document)
    where TDocument : IEditorDocument;

  void OpenDocument<TDocument>(VirtualPath file)
    where TDocument : IEditorDocument;

  void SaveDocument<TDocument>(VirtualPath file)
    where TDocument : IEditorDocument;

  void CloseDocument<TDocument>()
    where TDocument : IEditorDocument;

  void CloseDocument<TDocument>(TDocument document)
    where TDocument : IEditorDocument;
}

/// <summary>
/// Allows configuring the workload for the editor.
/// </summary>
public interface IEditorWorkloadConfiguration
{
  void RegisterDocumentType<[MeansImplicitUse] TDocument>()
    where TDocument : IEditorDocument;

  void RegisterDocumentEditor<[MeansImplicitUse] TControl, TDocument>()
    where TControl : Control, IEditorControl<TDocument>
    where TDocument : IEditorDocument;

  void RegisterMenuCommand<[MeansImplicitUse] TCommand>(string name)
    where TCommand : IEditorCommand;
}
