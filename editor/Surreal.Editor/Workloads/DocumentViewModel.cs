using System;
using System.Windows;
using System.Windows.Input;
using Surreal.Editor.Internal;

namespace Surreal.Editor.Workloads
{
  public sealed class DocumentViewModel
  {
    private readonly IDocument? document;

    public DocumentViewModel(IDocument? document = default)
    {
      this.document = document;

      NewCommand    = new Command(OnNew);
      OpenCommand   = new Command(OnOpen);
      SaveCommand   = new Command(OnSave);
      SaveAsCommand = new Command(OnSaveAs);
      ReloadCommand = new Command(OnReload);
      ExitCommand   = new Command(OnExit);
    }

    public ICommand NewCommand    { get; }
    public ICommand OpenCommand   { get; }
    public ICommand SaveCommand   { get; }
    public ICommand SaveAsCommand { get; }
    public ICommand ReloadCommand { get; }
    public ICommand ExitCommand   { get; }

    private void OnNew()    => throw new NotImplementedException();
    private void OnOpen()   => throw new NotImplementedException();
    private void OnSave()   => throw new NotImplementedException();
    private void OnSaveAs() => throw new NotImplementedException();
    private void OnReload() => throw new NotImplementedException();
    private void OnExit()   => Application.Current.Shutdown();
  }
}