using Surreal.Editing.Projects;

Editor.Start(new EditorConfiguration
{
  WindowTitle = "Custom Editor",
  DefaultProject = new EditorProject(Environment.CurrentDirectory)
  {
    Host = ProjectHost.OutOfProcess(Assembly.Load("Fractals"))
  }
});
