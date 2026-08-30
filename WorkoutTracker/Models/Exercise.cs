namespace WorkoutTracker.Models;

public class Exercise
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int BodyPartId { get; set; }
    public BodyPart? BodyPart { get; set; }
    
    // 目標RPE（Week1のベース値。各Weekで自動調整される）
    public double BaseTargetRpe { get; set; }
    
    // レップ範囲（例: "4〜6", "8〜12"）
    public string TargetRepRange { get; set; } = string.Empty;
    
    // 計算用：レップ範囲の中央値を自動算出
    public int TargetMidRep 
    {
        get
        {
            if (string.IsNullOrWhiteSpace(TargetRepRange)) return 10;
            var parts = TargetRepRange.Replace("〜", "~").Replace("～", "~").Split('~');
            if (parts.Length == 2 && int.TryParse(parts[0].Trim(), out int min) && int.TryParse(parts[1].Trim(), out int max))
                return (min + max) / 2;
            if (int.TryParse(TargetRepRange.Trim(), out int exact)) return exact;
            return 10;
        }
    }

    // デフォルトセット数（Week1のベース値）
    public int DefaultSetCount { get; set; }
    
    // 重量の刻み幅（例: 2.5kg, 1.0kg）
    public double WeightIncrement { get; set; }
    
    // AMRAPを追跡する種目か？（ONならコンパウンド型RPEパターン＋e1RM計算）
    public bool IsAmrapTracked { get; set; }
    
    // AMRAP記録
    public double AmrapWeight { get; set; }
    public int AmrapRep { get; set; }
    public double AmrapE1RM { get; set; }
    
    // 参考%1RM（初回参考重量算出用。Excelの%1RM列に相当）
    public double ReferencePercent1RM { get; set; }
    
    // 基準重量（1RMまたは10RM。STARTシートの入力値に相当）
    public double BaseWeight { get; set; }

    // カスタムWeekProfileを使用するかどうか
    public bool IsCustomWeekProfile { get; set; }

    public ICollection<WeekProfile> WeekProfiles { get; set; } = new List<WeekProfile>();
}
