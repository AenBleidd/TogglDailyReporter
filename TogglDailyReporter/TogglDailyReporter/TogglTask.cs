using System;
using System.Collections.Generic;
using Toggl;

namespace TogglDailyReporter
{
  public class TogglTask
  {
    private List<TimeEntry> timeEntries;
    private string name;
    private string projectName;
    private long duration;
    private long adjusted;
    private bool isChecked;

    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    public long Duration
    {
      get { return duration; }
      set { duration = value; }
    }
    public long Adjusted
    {
      get { return adjusted; }
      set { adjusted = value; }
    }
    public bool IsChecked
    {
      get { return isChecked; }
      set { isChecked = value; }
    }
    public string ProjectName
    {
      get { return projectName; }
      set { projectName = value; }
    }

    public TogglTask(string taskName, string project, List<TimeEntry> entries)
    {
      Name = taskName;
      projectName = project;
      duration = 0;
      timeEntries = new List<TimeEntry>();
      foreach (var e in entries)
      {
        if (e.Description == taskName && e.Duration != null)
        {
          duration += Convert.ToInt64(e.Duration);
          timeEntries.Add(e);
        }
      }
      adjusted = duration;
      isChecked = true;
    }

  }
}
