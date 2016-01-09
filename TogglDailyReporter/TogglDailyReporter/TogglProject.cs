using Toggl;

namespace TogglDailyReporter
{
  public class TogglProject
  {
    private Project project;
    private string name;
    private int? id;
    private bool isChecked;
    private object viewModel;

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
      set
      {
        if (isChecked == value) return;
        isChecked = value;
        var vm = viewModel as ReportViewModel;
        if (vm == null) return;
        vm.GetTasks(false);
      }
    }

    public TogglProject(Project project, object viewModel)
    {
      this.viewModel = viewModel;
      this.project = project;
      name = project.Name;
      id = project.Id;
      isChecked = true;
    }
  }
}
