using Surreal.Editing.Projects;

Editor.Start(new EditorConfiguration
{
  WindowTitle = "Basic Scene",
  DefaultProject = new Project(AppDomain.CurrentDomain.BaseDirectory)
  {
    EntryPoint = ProjectEntryPoint.FromAssembly(Assembly.LoadFrom("BasicScene"))
  }
});
