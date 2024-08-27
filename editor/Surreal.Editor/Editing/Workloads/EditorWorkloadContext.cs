using System.Windows.Input;
using Avalonia.Controls;

namespace Surreal.Editing.Workloads;

/// <summary>
/// The configured context for the editor workloads.
/// </summary>
public sealed class EditorWorkloadContext : IEditorWorkloadConfiguration
{
  private readonly HashSet<Type> _documentTypes = [];
  private readonly Dictionary<Type, IEditorDocumentFormat> _formats = new();
  private readonly Dictionary<Type, Type> _documentEditors = new();
  private readonly List<MenuCommand> _menuCommands = [];

  public EditorWorkloadContext(EditorConfiguration configuration)
  {
    foreach (var workload in configuration.Workloads)
    {
      workload.Configure(this);
    }
  }

  /// <summary>
  /// All the document types that are registered.
  /// </summary>
  public IEnumerable<Type> DocumentTypes => _documentTypes;

  /// <summary>
  /// All the menu commands that are registered.
  /// </summary>
  public IEnumerable<MenuCommand> MenuCommands => _menuCommands;

  public void RegisterDocumentType<TDocument>()
    where TDocument : IEditorDocument
  {
    if (_documentTypes.Add(typeof(TDocument)))
    {
      _formats.Add(typeof(TDocument), TDocument.Format);
    }
  }

  public void RegisterDocumentEditor<TControl, TDocument>()
    where TControl : Control, IEditorControl<TDocument> where TDocument : IEditorDocument
  {
    if (!_documentTypes.Contains(typeof(TDocument)))
    {
      throw new InvalidOperationException($"Document type {typeof(TDocument)} is not registered.");
    }

    _documentEditors.Add(typeof(TDocument), typeof(TControl));
  }

  public void RegisterMenuCommand<TCommand>(string name)
    where TCommand : IEditorCommand
  {
    _menuCommands.Add(new MenuCommand(name, typeof(TCommand)));
  }

  /// <summary>
  /// A command that can be executed in the menu.
  /// </summary>
  public sealed record MenuCommand(string Name, Type Type) : ICommand
  {
    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
      return true;
    }

    public void Execute(object? parameter)
    {
      var instance = (IEditorCommand)Activator.CreateInstance(Type)!;

      throw new NotImplementedException();
    }
  }
}
