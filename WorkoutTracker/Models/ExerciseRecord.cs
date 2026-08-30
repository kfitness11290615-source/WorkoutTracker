namespace WorkoutTracker.Models;

public class ExerciseRecord
{
    public int Id { get; set; }
    public int TrainingSessionId { get; set; }
    public TrainingSession? Session { get; set; }
    
    public int ExerciseId { get; set; }
    public Exercise? Exercise { get; set; }
    
    public int SetNumber { get; set; }
    public double Weight { get; set; }
    public int Rep { get; set; }
    
    public double Rpe { get; set; }
}
