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
    private object viewModel;

    public static string ConvertTime(long time)
    {
      int seconds = Convert.ToInt32(time);
      int hours = seconds / 3600;
      seconds -= (hours * 3600);
      int minutes = seconds / 60;
      seconds -= (minutes * 60);
      if (seconds != 0)
        minutes += 1;
      return Convert.ToString(hours) + "h " + Convert.ToString(minutes) + "m";
    }

    public string Name
    {
      get { return name; }
      set { name = value; }
    }
    public string DurationStr
    {
      get { return ConvertTime(Duration); }
    }
    public string AdjustedStr
    {
      get { return ConvertTime(Adjusted); }
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
        vm.GetTotalTime();
        vm.GetTotalTimeAdjusted();
      }
    }
    public string ProjectName
    {
      get { return projectName; }
      set { projectName = value; }
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

    public TogglTask(string taskName, string project, List<TimeEntry> entries, object viewModel)
    {
      this.viewModel = viewModel;
      Name = taskName;
      projectName = project;
      Duration = 0;
      timeEntries = new List<TimeEntry>();
      foreach (var e in entries)
      {
        if (e.Description == taskName && e.Duration != null)
        {
          Duration += Convert.ToInt64(e.Duration);
          timeEntries.Add(e);
        }
      }
      Adjusted = Duration;
      isChecked = true;
      Adjust();
    }
    public void Adjust(long seconds = 30 * 60)
    {
      Adjusted = AdjustTime(Adjusted, seconds);
    }
    public static long AdjustTime(long exact, long seconds = 30 * 60)
    {
      var adjustedSeconds = (seconds - exact % seconds);
      if (adjustedSeconds <= seconds / 2)
        return (exact += adjustedSeconds);
      else
        return (exact -= seconds - adjustedSeconds);
    }

  }
}
