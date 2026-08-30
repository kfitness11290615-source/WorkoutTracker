namespace WorkoutTracker.Models;

/// <summary>
/// 6週サイクルの各Weekにおける種目のRPE・セット数のプロファイル
/// </summary>
public class WeekProfile
{
    public int Id { get; set; }
    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }
    
    /// <summary>サイクル内の週番号 (1〜6)</summary>
    public int WeekNumber { get; set; }
    
    /// <summary>この週の目標RPE</summary>
    public double TargetRpe { get; set; }
    
    /// <summary>この週のセット数</summary>
    public int TargetSets { get; set; }
    
    /// <summary>この週のレップ範囲（空ならExerciseのデフォルトを使用）</summary>
    public string RepRange { get; set; } = string.Empty;
}
