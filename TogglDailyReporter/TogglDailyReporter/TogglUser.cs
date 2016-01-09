using System;

namespace TogglDailyReporter
{
  class TogglUser
  {
    private float workHours = 8.0f;
    private DateTime selectedDate = DateTime.Today;
    private string apiToken = string.Empty;

    protected TogglUser()
    {
      apiToken = TogglSettings.Default.ApiToken;
      workHours = TogglSettings.Default.WorkHours;
    }

    private sealed class TogglUserCreator
    {
      private static readonly TogglUser instance = new TogglUser();
      public static TogglUser Instance { get { return instance; } }
    }

    public static TogglUser Instance
    {
      get { return TogglUserCreator.Instance; }
    }

    ~TogglUser()
    {
      TogglSettings.Default.ApiToken = apiToken;
      TogglSettings.Default.WorkHours = workHours;
      TogglSettings.Default.Save();
    }

    public float WorkHours
    {
      get { return workHours; }
      set { workHours = value; }
    }
    public DateTime SelectedDate
    {
      get { return selectedDate; }
      set { selectedDate = value; }
    }
    public string ApiToken
    {
      get { return apiToken; }
      set { apiToken = value; }
    }
  }
}
