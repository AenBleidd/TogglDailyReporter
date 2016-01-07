using System;

namespace TogglDailyReporter
{
  public class ReportViewModel : NotifyPropertyChanged
  {
    private float workHours = 8.0f;
    private DateTime selectedDate = DateTime.Today;

    public string WorkHours
    {
      get { return Convert.ToString(workHours); }
      set { workHours = Convert.ToSingle(value); }
    }
    public DateTime SelectedDate
    {
      get { return selectedDate; }
      set { selectedDate = value; }
    }
  }
}
