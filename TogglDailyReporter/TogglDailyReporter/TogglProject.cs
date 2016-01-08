using Toggl;

namespace TogglDailyReporter
{
  public class TogglProject
  {
    private Project project;
    private string name;
    private int? id;
    private bool isChecked;

    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    public int? Id
    {
      get { return id; }
      set { id = value; }
    }
    public bool IsChecked
    {
      get { return isChecked; }
      set { isChecked = value; }
    }

    public TogglProject(Project project)
    {
      this.project = project;
      name = project.Name;
      id = project.Id;
      isChecked = false;
    }
  }
}
