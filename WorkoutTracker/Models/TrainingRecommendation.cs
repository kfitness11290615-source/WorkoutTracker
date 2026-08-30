using System;

namespace WorkoutTracker.Models;

public class TrainingRecommendation
{
    public double RecommendedWeight { get; set; }
    public double TargetRpe { get; set; }
    public int TargetSets { get; set; }
    public string RepRange { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string Confidence { get; set; } = string.Empty;
    public int CurrentWeek { get; set; }
    public string WeekPurpose { get; set; } = string.Empty;
    public bool IsAmrapWeek { get; set; }
    
    // 前回の実績
    public double? LastWeight { get; set; }
    public int? LastRep { get; set; }
    public double? LastRpe { get; set; }
    public DateTime? LastDate { get; set; }
}
