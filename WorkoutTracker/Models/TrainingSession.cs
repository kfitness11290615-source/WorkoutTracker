using System;
using System.Collections.Generic;

namespace WorkoutTracker.Models;

public class TrainingSession
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public string Memo { get; set; } = string.Empty;
    
    public int CycleNumber { get; set; }
    public int WeekInCycle { get; set; }
    
    public ICollection<ExerciseRecord> Records { get; set; } = new List<ExerciseRecord>();
}
