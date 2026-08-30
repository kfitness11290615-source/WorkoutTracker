namespace WorkoutTracker.Models;

public class GlobalWeekProfile
{
    public int Id { get; set; }
    public bool IsAmrapTracked { get; set; }
    public int WeekNumber { get; set; }
    public double TargetRpe { get; set; }
    public int TargetSets { get; set; }
    public string RepRange { get; set; } = string.Empty;
}
