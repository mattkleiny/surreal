using System.Collections.Generic;
using Surreal.Framework.Screens;

namespace Surreal.Diagnostics.Editing {
  public interface IEditableScreen : IScreen {
    void GetEditorProperties(ICollection<EditorProperty> properties);
  }
}