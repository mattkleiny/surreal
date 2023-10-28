using Surreal.Editing.Projects;

Editor.Start(new EditorConfiguration
{
  WindowTitle = "Custom Editor",
  DefaultProject = new EditorProject(Environment.CurrentDirectory, "BasicScene.csproj")
  {
    ProjectHost = ProjectHost.InProcess(Assembly.Load("BasicScene"))
  }
});
