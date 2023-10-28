using Surreal.Editing.Projects;

Editor.Start(new EditorConfiguration
{
  WindowTitle = "Custom Editor",
  DefaultProject = new EditorProject(Environment.CurrentDirectory, "BasicScene.csproj")
  {
    Host = ProjectHost.InProcess(Assembly.Load("BasicScene"))
  }
});
