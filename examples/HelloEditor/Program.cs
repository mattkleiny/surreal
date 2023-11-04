using Surreal.Editing.Projects;

Editor.Start(new EditorConfiguration
{
  WindowTitle = "Custom Editor",
  DefaultProject = new EditorProject(Environment.CurrentDirectory)
  {
    Host = ProjectHost.InProcess(Assembly.Load("Fractals"))
  }
});
