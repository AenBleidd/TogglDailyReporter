using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Toggl.QueryObjects;

namespace TogglDailyReporter
{
  public class ReportViewModel : NotifyPropertyChanged
  {
    private Toggl.Toggl toggl = null;
    private List<TogglProject> togglProjects = null;
    private List<TogglTask> togglTasks = null;
    private long totalTime = 0;
    private long totalTimeAdjusted = 0;

    public ReportViewModel()
    {
      btnLoginClick = new RelayCommand(Login);
      btnCopyToClipboardClick = new RelayCommand(CopyToClipboard);
      btnSaveAsCSVClick = new RelayCommand(SaveAsCSV);
      if (TogglUser.Instance.ApiToken != string.Empty)
        toggl = new Toggl.Toggl(TogglUser.Instance.ApiToken);
      GetReport(true);
    }

    public ICommand btnLoginClick
    {
      get;
      private set;
    }
    public ICommand btnCopyToClipboardClick
    {
      get;
      private set;
    }
    public ICommand btnSaveAsCSVClick
    {
      get;
      private set;
    }

    public string WorkHours
    {
      get { return Convert.ToString(TogglUser.Instance.WorkHours); }
      set { TogglUser.Instance.WorkHours = Convert.ToSingle(value); }
    }
    public DateTime SelectedDate
    {
      get { return TogglUser.Instance.SelectedDate; }
      set
      {
        if (TogglUser.Instance.SelectedDate == value) return;
        TogglUser.Instance.SelectedDate = value;
        GetReport(false);
      }
    }
    public string ApiToken
    {
      get { return TogglUser.Instance.ApiToken; }
      set { TogglUser.Instance.ApiToken = value; }
    }
    public List<TogglProject> TogglProjects
    {
      get { return togglProjects; }
      set { togglProjects = value; }
    }
    public List<TogglTask> TogglTasks
    {
      get { return togglTasks; }
      set { togglTasks = value; }
    }
    public long TotalTime
    {
      get { return totalTime; }
      set { totalTime = value; }
    }
    public long TotalTimeAdjusted
    {
      get { return totalTimeAdjusted; }
      set { totalTimeAdjusted = value; }
    }
    public string TotalTimeStr
    {
      get { return TogglTask.ConvertTime(totalTime); }
    }
    public string TotalTimeAdjustedStr
    {
      get { return TogglTask.ConvertTime(totalTimeAdjusted); }
    }

    private void Login(object sender)
    {
      if (ApiToken == string.Empty)
      {
        MessageBox.Show("Api Token can't be empty!", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }
      toggl = new Toggl.Toggl(ApiToken);
      GetReport(false);
    }
    private void GetProjects(bool bSilent = false)
    {
      try
      {
        if (toggl != null)
        {
          var projects = toggl.Project.List();
          togglProjects = new List<TogglProject>();
          foreach (var p in projects)
            togglProjects.Add(new TogglProject(p, this));
          togglProjects.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
          OnPropertyChanged("TogglProjects");
        }
      }
      catch (Exception ex)
      {
        if (!bSilent)
          MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
      }
    }
    public void GetTasks(bool bSilent = false)
    {
      try
      {
        if (toggl != null)
        {
          var p = new TimeEntryParams();
          p.StartDate = SelectedDate;
          p.EndDate = SelectedDate.AddDays(1);
          var e = toggl.TimeEntry.List(p);
          var d = new Dictionary<string, string>();
          var pr = new List<long>();
          foreach (var t in e)
          {
            if (t.Description != string.Empty && t.Description != null && !d.ContainsKey(t.Description) && t.Duration > 0)
            {
              string project = string.Empty;
              bool bChecked = false;
              foreach (var i in togglProjects)
              {
                if (i.Id == t.ProjectId)
                {
                  project = i.Name;
                  bChecked = i.IsChecked;
                  break;
                }
              }
              if (bChecked)
                d.Add(t.Description, project);
              if (!pr.Contains(Convert.ToInt64(t.ProjectId)))
                pr.Add(Convert.ToInt64(t.ProjectId));
            }
          }
          togglTasks = new List<TogglTask>();
          foreach (var t in d)
            togglTasks.Add(new TogglTask(t.Key, t.Value, e, this));
          togglTasks.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));

          foreach (var t in togglProjects.ToArray())
          {
            if (!pr.Contains(Convert.ToInt64((t.Id))))
              togglProjects.Remove(t);
          }
          GetTotalTime();
          GetTotalTimeAdjusted();
          OnPropertyChanged("TogglTasks");
          OnPropertyChanged("TogglProjects");
        }
      }
      catch (Exception ex)
      {
        if (!bSilent)
          MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
      }
}
    private void GetReport(bool bSilent = false)
    {
      GetProjects(bSilent);
      GetTasks(bSilent);
    }
    public void GetTotalTime()
    {
      if (togglTasks == null) return;
      totalTime = 0;
      foreach (var t in togglTasks)
        if (t.IsChecked)
          totalTime += t.Duration;
      OnPropertyChanged("TotalTimeStr");
    }
    public void GetTotalTimeAdjusted()
    {
      totalTimeAdjusted = TogglTask.AdjustTime(totalTime);
      OnPropertyChanged("TotalTimeAdjustedStr");
    }
    private string GetReport()
    {
      var str = string.Empty;
      foreach (var t in togglTasks)
        if (t.IsChecked)
          str += t.Name + ": " + t.AdjustedStr + Environment.NewLine;
      str += "Total: " + TotalTimeAdjustedStr + Environment.NewLine;
      return str;
    }
    private string GetReportCSV()
    {
      var str = string.Empty;
      foreach (var t in togglTasks)
        if (t.IsChecked)
          str += t.Name + ';' + t.AdjustedStr + ';' + Environment.NewLine;
      str += "Total;" + TotalTimeAdjustedStr + ';' + Environment.NewLine;
      return str;
    }
    private void CopyToClipboard(object sender)
    {
      Clipboard.SetText(GetReport());
    }
    private void SaveAsCSV(object sender)
    {
      var dlg = new SaveFileDialog();
      dlg.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*";
      if (dlg.ShowDialog() == true)
        File.WriteAllText(dlg.FileName, GetReportCSV());
    }
  }
}
