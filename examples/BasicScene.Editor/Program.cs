using Surreal.Editing.Projects;

Editor.Start(new EditorConfiguration
{
  WindowTitle = "Custom Editor",
  DefaultProject = Project.Load(Environment.CurrentDirectory, "BasicScene.csproj")
});
