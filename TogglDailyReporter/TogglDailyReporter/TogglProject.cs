using System;

using Toggl;

namespace TogglDailyReporter
{
  class TogglProject
  {
    private Project project;
    private DateTime date;

    public TogglProject(Project project, DateTime date)
    {
      this.project = project;
      this.date = date;
    }
  }
}
