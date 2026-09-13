using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;
using WorkoutTracker.Services;

namespace WorkoutTracker.Components.Pages;

public partial class TrainingLog
{
    [Inject] private IDbContextFactory<AppDbContext> DbFactory { get; set; } = null!;
    [Inject] private TrainingRecommendationService RecommendationService { get; set; } = null!;

    [Parameter]
    public string? DateString { get; set; }

    private DateTime CurrentDate;
    private string FormattedDate => CurrentDate.ToString("yyyy/MM/dd (ddd)");
    
    private TrainingSession? CurrentSession;
    private List<BodyPart> BodyParts = new();
    private List<Exercise> AllExercises = new();

    // 追加モーダル用
    private bool ShowAddModal;
    private int AddStep = 1;
    private BodyPart? SelectedBodyPartForAdd;
    private List<Exercise> ModalFilteredExercises = new();

    // アクティブな種目ウィジェット一覧
    private List<ExerciseWidget> ActiveExercises = new();

    // 過去履歴モーダル用
    private bool ShowHistoryModal;
    private bool IsLoadingHistory;
    private Exercise? HistoryExercise;
    private List<ExerciseHistoryGroup> HistoryGroups = new();

    protected override async Task OnInitializedAsync()
    {
        if (string.IsNullOrEmpty(DateString) || !DateTime.TryParse(DateString, out CurrentDate))
        {
            CurrentDate = DateTime.Today;
        }

        await using var context = await DbFactory.CreateDbContextAsync();
        
        BodyParts = await context.BodyParts.ToListAsync();
        AllExercises = await context.Exercises.Include(e => e.BodyPart).ToListAsync();
        
        // 既存のセッションがあればロード（なければ null のまま保持し、最初の記録保存時に作成する）
        CurrentSession = await context.TrainingSessions.FirstOrDefaultAsync(s => s.Date.Date == CurrentDate.Date);
        if (CurrentSession != null)
        {
            // 既存の記録がある日はウィジェットを自動復元
            var todayRecords = await context.ExerciseRecords
                .Where(r => r.TrainingSessionId == CurrentSession.Id)
                .OrderBy(r => r.SetNumber)
                .ToListAsync();

            var exerciseIds = todayRecords.Select(r => r.ExerciseId).Distinct();
            foreach (var exId in exerciseIds)
            {
                var exercise = AllExercises.FirstOrDefault(e => e.Id == exId);
                if (exercise != null)
                {
                    var widget = new ExerciseWidget { Exercise = exercise };
                    widget.Records = todayRecords.Where(r => r.ExerciseId == exId).ToList();
                    await LoadWidgetRecommendation(widget);
                    ActiveExercises.Add(widget);
                }
            }
        }
    }

    // === セッション遅延作成 ===
    /// <summary>
    /// TrainingSession が未作成の場合に作成します。
    /// ページを開いただけでは作成せず、最初のレコード保存時に呼ばれます。
    /// </summary>
    private async Task EnsureSessionExistsAsync()
    {
        if (CurrentSession != null) return;

        await using var context = await DbFactory.CreateDbContextAsync();
        CurrentSession = new TrainingSession { Date = CurrentDate, WeekInCycle = 1 };
        context.TrainingSessions.Add(CurrentSession);
        await context.SaveChangesAsync();
    }

    // === FABモーダル関連 ===
    private void OpenAddModal()
    {
        ShowAddModal = true;
        AddStep = 1;
    }

    private void CloseAddModal()
    {
        ShowAddModal = false;
        AddStep = 1;
    }

    private void SelectBodyPartForAdd(int bodyPartId)
    {
        SelectedBodyPartForAdd = BodyParts.FirstOrDefault(b => b.Id == bodyPartId);
        ModalFilteredExercises = AllExercises.Where(e => e.BodyPartId == bodyPartId).ToList();
        AddStep = 2;
    }

    private async Task AddExerciseToSession(Exercise exercise)
    {
        if (ActiveExercises.Any(a => a.Exercise.Id == exercise.Id)) return;

        var widget = new ExerciseWidget
        {
            Exercise = exercise,
            IsExpanded = true
        };

        await LoadWidgetRecommendation(widget);

        // 推奨値をフォームの初期値に設定
        if (widget.Recommendation != null)
        {
            widget.DynamicRecommendedWeight = widget.Recommendation.RecommendedWeight;
            widget.NewWeight = widget.Recommendation.RecommendedWeight;
            widget.NewRpe = widget.Recommendation.TargetRpe;
            var repParts = widget.Recommendation.RepRange?.Replace("〜", "~").Replace("～", "~").Split('~');
            if (repParts?.Length > 0 && int.TryParse(repParts[0].Trim(), out int startRep))
                widget.NewRep = startRep;
        }

        ActiveExercises.Add(widget);
        ShowAddModal = false;
    }

    private async Task LoadWidgetRecommendation(ExerciseWidget widget)
    {
        await using var context = await DbFactory.CreateDbContextAsync();

        // 前回記録を取得（前回セッションの全セット）
        var lastRec = await context.ExerciseRecords
            .Include(r => r.Session)
            .Where(r => r.ExerciseId == widget.Exercise.Id && r.Session!.Date.Date < CurrentDate.Date)
            .OrderByDescending(r => r.Session!.Date)
            .FirstOrDefaultAsync();

        if (lastRec != null)
        {
            // 前回セッション日の全セットを取得
            var lastSessionDate = lastRec.Session!.Date.Date;
            widget.LastRecords = await context.ExerciseRecords
                .Include(r => r.Session)
                .Where(r => r.ExerciseId == widget.Exercise.Id && r.Session!.Date.Date == lastSessionDate)
                .OrderBy(r => r.SetNumber)
                .ToListAsync();

            widget.LastRecord = lastRec;
            widget.LastRecordDate = lastSessionDate.ToString("yyyy/MM/dd");
        }

        widget.Recommendation = await RecommendationService.GetRecommendationAsync(widget.Exercise, lastRec, CurrentDate);
        
        // 当日既に記録がある場合は、その最後のセットから次セットの推奨重量を計算する
        if (widget.Records.Any())
        {
            CalculateNextSetWeight(widget, widget.Records.OrderBy(r => r.SetNumber).Last());
        }
        else
        {
            widget.DynamicRecommendedWeight = widget.Recommendation?.RecommendedWeight ?? 0;
        }
    }

    // === ウィジェット操作 ===
    private void ToggleWidget(ExerciseWidget widget)
    {
        widget.IsExpanded = !widget.IsExpanded;
    }

    private async Task SaveRecord(ExerciseWidget widget)
    {
        // セッションが未作成なら作成（遅延作成パターン）
        await EnsureSessionExistsAsync();

        await using var context = await DbFactory.CreateDbContextAsync();
        
        int setNum = widget.Records.Count > 0 ? widget.Records.Max(r => r.SetNumber) + 1 : 1;

        var record = new ExerciseRecord
        {
            TrainingSessionId = CurrentSession!.Id,
            ExerciseId = widget.Exercise.Id,
            SetNumber = setNum,
            Weight = widget.NewWeight,
            Rep = widget.NewRep,
            Rpe = widget.NewRpe
        };

        context.ExerciseRecords.Add(record);

        // AMRAPの自動更新ロジック
        if (widget.Exercise.IsAmrapTracked && widget.Recommendation?.IsAmrapWeek == true)
        {
            var exToUpdate = await context.Exercises.FindAsync(widget.Exercise.Id);
            if (exToUpdate != null)
            {
                double e1RM = E1RMCalculator.Calculate(record.Weight, record.Rep, record.Rpe);
                
                // より高い推定1RM（または初回）なら設定画面のAMRAP記録と基準重量を自動更新する
                if (exToUpdate.AmrapE1RM < e1RM || exToUpdate.AmrapE1RM == 0)
                {
                    exToUpdate.AmrapWeight = record.Weight;
                    exToUpdate.AmrapRep = record.Rep;
                    exToUpdate.AmrapE1RM = e1RM;
                    
                    // 次サイクル向けの基準重量も新しい記録に合わせて更新
                    double inc = exToUpdate.WeightIncrement > 0 ? exToUpdate.WeightIncrement : 1;
                    if (exToUpdate.ReferencePercent1RM > 0)
                    {
                        exToUpdate.BaseWeight = E1RMCalculator.RoundToIncrement(
                            e1RM * exToUpdate.ReferencePercent1RM, inc);
                    }
                    else
                    {
                        exToUpdate.BaseWeight = E1RMCalculator.RoundToIncrement(e1RM * 0.75, inc);
                    }

                    // UI上の表示も更新
                    widget.Exercise.AmrapWeight = exToUpdate.AmrapWeight;
                    widget.Exercise.AmrapRep = exToUpdate.AmrapRep;
                    widget.Exercise.AmrapE1RM = exToUpdate.AmrapE1RM;
                    widget.Exercise.BaseWeight = exToUpdate.BaseWeight;
                }
            }
        }

        await context.SaveChangesAsync();

        widget.Records.Add(record);

        // 次セットの重量を再計算
        CalculateNextSetWeight(widget, record);
    }

    /// <summary>
    /// 次回参考重量の算出
    /// </summary>
    /// <param name="widget"></param>
    /// <param name="lastSetRecord"></param>
    private void CalculateNextSetWeight(ExerciseWidget widget, ExerciseRecord lastSetRecord)
    {
        if (widget.Exercise.IsAmrapTracked && widget.Recommendation != null)
        {
            double e1RM = E1RMCalculator.Calculate(lastSetRecord.Weight, lastSetRecord.Rep, lastSetRecord.Rpe);
            int midRep = widget.Exercise.TargetMidRep;
            double targetRpe = widget.Recommendation.TargetRpe;
            double increment = widget.Exercise.WeightIncrement > 0 ? widget.Exercise.WeightIncrement : 1.0;
            
            // 次セットの推奨重量を計算
            widget.DynamicRecommendedWeight = E1RMCalculator.RoundToIncrement(
                E1RMCalculator.ReverseWeight(e1RM, midRep, targetRpe), increment);
            
            // フォームの値も自動更新
            // widget.NewWeight = widget.DynamicRecommendedWeight;
            // widget.NewRpe = targetRpe;
        }
        else
        {
            // AMRAP対象外の場合は、同じ重量をキープ
            widget.DynamicRecommendedWeight = lastSetRecord.Weight;
            widget.NewWeight = lastSetRecord.Weight;
        }
    }

    private async Task DeleteRecord(ExerciseWidget widget, int recordId)
    {
        await using var context = await DbFactory.CreateDbContextAsync();
        var record = await context.ExerciseRecords.FindAsync(recordId);
        if (record != null)
        {
            context.ExerciseRecords.Remove(record);
            await context.SaveChangesAsync();
            widget.Records.RemoveAll(r => r.Id == recordId);
        }
    }

    private async Task RemoveExerciseWidget(ExerciseWidget widget)
    {
        // 記録がある場合はDBからも削除する
        if (widget.Records.Any() && CurrentSession != null)
        {
            await using var context = await DbFactory.CreateDbContextAsync();
            var recordsToRemove = await context.ExerciseRecords
                .Where(r => r.TrainingSessionId == CurrentSession.Id && r.ExerciseId == widget.Exercise.Id)
                .ToListAsync();
            
            if (recordsToRemove.Any())
            {
                context.ExerciseRecords.RemoveRange(recordsToRemove);
                await context.SaveChangesAsync();
            }
        }
        
        ActiveExercises.Remove(widget);
    }

    // === 過去履歴モーダル関連 ===
    private async Task OpenHistoryModal(Exercise exercise)
    {
        HistoryExercise = exercise;
        ShowHistoryModal = true;
        IsLoadingHistory = true;
        HistoryGroups.Clear();

        await using var context = await DbFactory.CreateDbContextAsync();

        var records = await context.ExerciseRecords
            .Include(r => r.Session)
            .Where(r => r.ExerciseId == exercise.Id && r.Session != null && r.Session.Date.Date < CurrentDate.Date)
            .OrderByDescending(r => r.Session!.Date)
            .ThenBy(r => r.SetNumber)
            .ToListAsync();

        if (records.Count > 0)
        {
            // 種目に紐づく部位のトレーニング頻度（週あたり日数）を取得
            var bodyPart = await context.BodyParts.FindAsync(exercise.BodyPartId);
            int daysPerWeek = (bodyPart != null && bodyPart.TrainingDaysPerWeek > 0) ? bodyPart.TrainingDaysPerWeek : 1;

            var relatedExerciseIds = await context.Exercises
                .Where(e => e.BodyPartId == exercise.BodyPartId)
                .Select(e => e.Id)
                .ToListAsync();

            // この部位のトレーニングが含まれる全セッション日（昇順・ユニーク）を取得
            var sessionDates = await context.ExerciseRecords
                .Where(r => relatedExerciseIds.Contains(r.ExerciseId) && r.Session != null)
                .Select(r => r.Session!.Date.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            var dateToWeekMap = new Dictionary<DateTime, (int Week, int Cycle)>();
            for (int i = 0; i < sessionDates.Count; i++)
            {
                int sessionIndex = i + 1;
                int week = ((sessionIndex - 1) / daysPerWeek) % 6 + 1;
                int cycle = ((sessionIndex - 1) / (daysPerWeek * 6)) + 1;
                dateToWeekMap[sessionDates[i]] = (week, cycle);
            }

            HistoryGroups = records
                .GroupBy(r => r.Session!.Date.Date)
                .Select(g =>
                {
                    int week = 1;
                    int cycle = 1;
                    if (dateToWeekMap.TryGetValue(g.Key, out var val))
                    {
                        week = val.Week;
                        cycle = val.Cycle;
                    }

                    return new ExerciseHistoryGroup
                    {
                        Date = g.Key,
                        CycleNumber = cycle,
                        WeekInCycle = week,
                        WeekPurpose = GetWeekPurpose(week),
                        Records = g.ToList(),
                        BestE1RM = g.Max(r => E1RMCalculator.Calculate(r.Weight, r.Rep, r.Rpe))
                    };
                })
                .ToList();
        }

        IsLoadingHistory = false;
    }

    private static string GetWeekPurpose(int week) => week switch
    {
        1 => "漸進①",
        2 => "漸進②",
        3 => "漸進③",
        4 => "高強度",
        5 => "AMRAP評価",
        6 => "疲労抜き",
        _ => ""
    };

    private void CloseHistoryModal()
    {
        ShowHistoryModal = false;
        HistoryExercise = null;
        HistoryGroups.Clear();
    }

    // === ウィジェットモデル ===
    private class ExerciseWidget
    {
        public Exercise Exercise { get; set; } = null!;
        public bool IsExpanded { get; set; } = true;
        public TrainingRecommendation? Recommendation { get; set; }
        public ExerciseRecord? LastRecord { get; set; }
        public List<ExerciseRecord> LastRecords { get; set; } = new();
        public string LastRecordDate { get; set; } = "";
        
        public double DynamicRecommendedWeight { get; set; }

        public List<ExerciseRecord> Records { get; set; } = new();
        public double NewWeight { get; set; }
        public int NewRep { get; set; }
        public double NewRpe { get; set; }
    }

    // === 履歴表示用グループモデル ===
    private class ExerciseHistoryGroup
    {
        public DateTime Date { get; set; }
        public int CycleNumber { get; set; }
        public int WeekInCycle { get; set; }
        public string WeekPurpose { get; set; } = "";
        public List<ExerciseRecord> Records { get; set; } = new();
        public double BestE1RM { get; set; }
    }
}
