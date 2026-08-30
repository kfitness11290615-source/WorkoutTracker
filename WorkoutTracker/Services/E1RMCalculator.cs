using System;

namespace WorkoutTracker.Services;

/// <summary>
/// 推定1RM (e1RM) の計算ユーティリティ。
/// Epley式ベースの RPE 調整版: e1RM = weight * (1 + (rep + 10 - RPE) / 30)
/// </summary>
public static class E1RMCalculator
{
    /// <summary>
    /// 推定1RM (e1RM) を計算します。
    /// </summary>
    /// <param name="weight">使用重量</param>
    /// <param name="rep">実施レップ数</param>
    /// <param name="rpe">RPE (主観的運動強度)</param>
    /// <returns>推定1RM</returns>
    public static double Calculate(double weight, int rep, double rpe)
        => weight * (1.0 + (rep + 10.0 - rpe) / 30.0);

    /// <summary>
    /// e1RM から目標レップ数・RPE に対応する推奨重量を逆算します。
    /// </summary>
    /// <param name="e1rm">推定1RM</param>
    /// <param name="midRep">目標レップ数の中央値</param>
    /// <param name="targetRpe">目標RPE</param>
    /// <returns>推奨重量（未丸め）</returns>
    public static double ReverseWeight(double e1rm, int midRep, double targetRpe)
        => e1rm / (1.0 + (midRep + 10.0 - targetRpe) / 30.0);

    /// <summary>
    /// 重量を指定の刻み幅に丸めます。
    /// </summary>
    /// <param name="value">丸め対象の値</param>
    /// <param name="increment">刻み幅（例: 2.5kg）</param>
    /// <returns>丸め後の値</returns>
    public static double RoundToIncrement(double value, double increment)
        => increment > 0 ? Math.Round(value / increment) * increment : Math.Round(value);
}
