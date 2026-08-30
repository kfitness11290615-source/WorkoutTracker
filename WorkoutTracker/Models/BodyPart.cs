namespace WorkoutTracker.Models;

public class BodyPart
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    
    // この部位の1週間あたりのトレーニング日数（WeekInCycle計算に使用）
    public int TrainingDaysPerWeek { get; set; } = 2;
}
