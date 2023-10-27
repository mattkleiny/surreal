namespace Surreal;

public static class Program
{
  [STAThread]
  public static int Main()
  {
    return Editor.Start(new EditorConfiguration());
  }
}
