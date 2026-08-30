using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Data;
using WorkoutTracker.Models;

namespace WorkoutTracker.Services
{
    /// <summary>
    /// 6週サイクルのピリオダイゼーションロジックに基づいてトレーニングの推奨値を提供するサービス
    /// </summary>
    public class TrainingRecommendationService
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="contextFactory">DBコンテキストファクトリ</param>
        public TrainingRecommendationService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        /// <summary>
        /// 指定された種目のトレーニング推奨値を取得します。
        /// </summary>
        /// <param name="exercise">対象の種目</param>
        /// <param name="lastRecord">前回記録（存在する場合）</param>
        /// <param name="targetDate">ターゲット日付</param>
        /// <returns>トレーニング推奨値を含む <see cref="TrainingRecommendation"/></returns>
        public async Task<TrainingRecommendation> GetRecommendationAsync(Exercise exercise, ExerciseRecord? lastRecord, DateTime targetDate)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            // 現在の週（1〜6）を計算
            int currentWeek = await GetCurrentWeekAsync(exercise, targetDate);

            double targetRpe = exercise.BaseTargetRpe;
            int targetSets = exercise.DefaultSetCount;
            string repRange = exercise.TargetRepRange;

            if (exercise.IsCustomWeekProfile)
            {
                var weekProfile = await context.WeekProfiles
                    .FirstOrDefaultAsync(wp => wp.ExerciseId == exercise.Id && wp.WeekNumber == currentWeek);
                
                if (weekProfile != null)
                {
                    targetRpe = weekProfile.TargetRpe;
                    targetSets = weekProfile.TargetSets;
                    if (!string.IsNullOrEmpty(weekProfile.RepRange))
                        repRange = weekProfile.RepRange;
                }
            }
            else
            {
                var globalProfile = await context.GlobalWeekProfiles
                    .FirstOrDefaultAsync(gp => gp.IsAmrapTracked == exercise.IsAmrapTracked && gp.WeekNumber == currentWeek);
                
                if (globalProfile != null)
                {
                    targetRpe = globalProfile.TargetRpe;
                    targetSets = globalProfile.TargetSets;
                    if (!string.IsNullOrEmpty(globalProfile.RepRange))
                        repRange = globalProfile.RepRange;
                }
            }

            double recommendedWeight = 0;
            bool isAmrapWeek = (currentWeek == 5);
            double increment = exercise.WeightIncrement > 0 ? exercise.WeightIncrement : 1.0; // 0除算防止用

            if (exercise.IsAmrapTracked)
            {
                if (lastRecord != null)
                {
                    // 前回記録が存在する場合の推定1RM(e1RM)の計算
                    double e1RM = E1RMCalculator.Calculate(lastRecord.Weight, lastRecord.Rep, lastRecord.Rpe);

                    if (isAmrapWeek)
                    {
                        // AMRAP週 (Week 5): 80%の推定1RMを使用
                        recommendedWeight = E1RMCalculator.RoundToIncrement(e1RM * 0.8, increment);
                    }
                    else
                    {
                        // 通常週: 目標RPEと対象の中間レップ数(TargetMidRep)から逆算
                        recommendedWeight = E1RMCalculator.RoundToIncrement(
                            E1RMCalculator.ReverseWeight(e1RM, exercise.TargetMidRep, targetRpe), increment);
                    }
                }
                else
                {
                    // 前回記録がない場合の初回設定
                    if (exercise.ReferencePercent1RM > 0)
                    {
                        // ReferencePercent1RMが設定されている場合はそれを使用
                        recommendedWeight = E1RMCalculator.RoundToIncrement(
                            exercise.BaseWeight * exercise.ReferencePercent1RM, increment);
                    }
                    else
                    {
                        // 10RMベースの場合のフォールバック (1.1倍)
                        // 基本重量は基本1RMを想定して0.8倍に変更
                        recommendedWeight = E1RMCalculator.RoundToIncrement(exercise.BaseWeight * 0.8, increment);
                    }
                }
            }
            else
            {
                // AMRAPでトラッキングしない場合、単に前回の重量を推奨とする
                recommendedWeight = lastRecord?.Weight ?? 0;
            }

            // 週の目的を決定
            string weekPurpose = currentWeek switch
            {
                1 => "漸進①",
                2 => "漸進②",
                3 => "漸進③",
                4 => "高強度",
                5 => "AMRAP評価",
                6 => "疲労抜き",
                _ => "不明"
            };

            return new TrainingRecommendation
            {
                RecommendedWeight = recommendedWeight,
                TargetRpe = targetRpe,
                TargetSets = targetSets,
                RepRange = repRange,
                CurrentWeek = currentWeek,
                WeekPurpose = weekPurpose,
                IsAmrapWeek = isAmrapWeek,
                Reason = isAmrapWeek ? "AMRAP週の評価基準に基づく推奨" : "現在のピリオダイゼーションフェーズに基づく推奨",
                Confidence = lastRecord != null ? "High" : "Low",
                LastWeight = lastRecord?.Weight,
                LastRep = lastRecord?.Rep,
                LastRpe = lastRecord?.Rpe,
                LastDate = lastRecord?.Session?.Date
            };
        }

        /// <summary>
        /// 現在のサイクルの週（1〜6）を計算します。
        /// 算出ロジック:
        /// 同じ部位のトレーニングセッション数を数え、部位毎の週あたりのトレーニング頻度で割り、
        /// 6週サイクルでの現在週（1〜6）を求めます。
        /// 式: ((distinctSessionCount / daysPerWeek) % 6) + 1
        /// </summary>
        /// <param name="exercise">対象の種目</param>
        /// <param name="targetDate">判定対象日（この日より前のセッションをカウント）</param>
        /// <returns>現在の週番号（1〜6）</returns>
        public async Task<int> GetCurrentWeekAsync(Exercise exercise, DateTime? targetDate = null)
        {
            await using var context = await _contextFactory.CreateDbContextAsync();

            // 種目に紐づく部位を取得 (ナビゲーションプロパティがロードされていない可能性があるためDBから取得)
            var bodyPart = await context.BodyParts.FindAsync(exercise.BodyPartId);
            if (bodyPart == null)
            {
                return 1; // 部位が存在しない場合は安全のため1週目を返す
            }

            int daysPerWeek = bodyPart.TrainingDaysPerWeek > 0 ? bodyPart.TrainingDaysPerWeek : 1;

            // 同じ部位に関連するエクササイズのIDリストを取得
            var relatedExerciseIds = await context.Exercises
                .Where(e => e.BodyPartId == exercise.BodyPartId)
                .Select(e => e.Id)
                .ToListAsync();

            // その部位のトレーニングが含まれるユニークなセッションの数をカウント
            var query = context.ExerciseRecords
                .Include(r => r.Session)
                .Where(r => relatedExerciseIds.Contains(r.ExerciseId));

            if (targetDate.HasValue)
            {
                var target = targetDate.Value.Date;
                query = query.Where(r => r.Session != null && r.Session.Date < target);
            }

            int pastSessionCount = await query
                .Select(r => r.TrainingSessionId)
                .Distinct()
                .CountAsync();
                
            // ターゲット日が指定されている場合は「過去の数＋今日の分(1)」とする
            int distinctSessionCount = targetDate.HasValue ? pastSessionCount + 1 : pastSessionCount;

            // 週数を計算（6週サイクル）
            int currentWeek = ((distinctSessionCount - 1) / daysPerWeek) % 6 + 1;
            
            // 例: daysPerWeek=2 の場合
            // distinctSessionCount = 1 -> (0/2)%6 + 1 = 1
            // distinctSessionCount = 2 -> (1/2)%6 + 1 = 1
            // distinctSessionCount = 3 -> (2/2)%6 + 1 = 2
            
            return currentWeek;
        }
    }
}
