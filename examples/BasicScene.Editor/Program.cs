using Surreal.Editing.Projects;

Editor.Start(new EditorConfiguration
{
  WindowTitle = "Custom Editor",
  DefaultProject = new Project(Environment.CurrentDirectory)
});
