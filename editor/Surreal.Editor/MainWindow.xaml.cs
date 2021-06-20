using JetBrains.Annotations;
using Surreal.Editor.Workloads;

namespace Surreal.Editor
{
  [UsedImplicitly]
  public partial class MainWindow
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = new DocumentViewModel();
    }
  }
}