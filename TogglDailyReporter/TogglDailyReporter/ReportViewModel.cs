using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Toggl;
using Toggl.QueryObjects;

namespace TogglDailyReporter
{
  public class ReportViewModel : NotifyPropertyChanged
  {
    private Toggl.Toggl toggl = null;
    private List<TogglProject> togglProjects = null;
    private List<TogglTask> togglTasks = null;

    public ReportViewModel()
    {
      btnLoginClick = new RelayCommand(Login);
      if (TogglUser.Instance.ApiToken != string.Empty)
        toggl = new Toggl.Toggl(TogglUser.Instance.ApiToken);
      GetReport(true);
    }

    public ICommand btnLoginClick
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

    public void Login(object sender)
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
            togglProjects.Add(new TogglProject(p));
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

    private void GetTasks(bool bSilent = false)
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
          foreach (var t in e)
          {
            if (t.Description != string.Empty && !d.ContainsKey(t.Description))
            {
              string project = string.Empty;
              foreach (var i in togglProjects)
              {
                if (i.Id == t.ProjectId)
                {
                  project = i.Name;
                  break;
                }
              }
              d.Add(t.Description, project);
            }
          }
          togglTasks = new List<TogglTask>();
          foreach (var t in d)
            togglTasks.Add(new TogglTask(t.Key, t.Value, e));

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

  }
}
