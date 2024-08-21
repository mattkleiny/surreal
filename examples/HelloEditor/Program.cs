using HelloEditor.Workloads;
using Surreal.Editing.Projects;

return Editor.Start(new EditorConfiguration
{
  WindowTitle = "Custom Editor",
  DefaultProject = new EditorProject(Environment.CurrentDirectory)
  {
    Host = ProjectHost.InProcess(Assembly.Load("HelloWorld"))
  },
  Workloads =
  [
    new TestWorkload()
  ]
});
