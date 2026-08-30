using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WorkoutTracker.Models;

namespace WorkoutTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<BodyPart> BodyParts { get; set; } = null!;
    public DbSet<Exercise> Exercises { get; set; } = null!;
    public DbSet<TrainingSession> TrainingSessions { get; set; } = null!;
    public DbSet<ExerciseRecord> ExerciseRecords { get; set; } = null!;
    public DbSet<WeekProfile> WeekProfiles { get; set; } = null!;
    public DbSet<GlobalWeekProfile> GlobalWeekProfiles { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Exercise>()
            .HasOne(e => e.BodyPart)
            .WithMany()
            .HasForeignKey(e => e.BodyPartId);

        modelBuilder.Entity<ExerciseRecord>()
            .HasOne(er => er.Session)
            .WithMany(s => s.Records)
            .HasForeignKey(er => er.TrainingSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ExerciseRecord>()
            .HasOne(er => er.Exercise)
            .WithMany()
            .HasForeignKey(er => er.ExerciseId);

        modelBuilder.Entity<WeekProfile>()
            .HasKey(wp => wp.Id);

        modelBuilder.Entity<WeekProfile>()
            .HasOne(wp => wp.Exercise)
            .WithMany(e => e.WeekProfiles)
            .HasForeignKey(wp => wp.ExerciseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed BodyParts
        modelBuilder.Entity<BodyPart>().HasData(
            new BodyPart { Id = 1, Name = "胸", TrainingDaysPerWeek = 2 },
            new BodyPart { Id = 2, Name = "背中", TrainingDaysPerWeek = 2 },
            new BodyPart { Id = 3, Name = "肩", TrainingDaysPerWeek = 2 },
            new BodyPart { Id = 4, Name = "脚", TrainingDaysPerWeek = 2 },
            new BodyPart { Id = 5, Name = "二頭", TrainingDaysPerWeek = 2 },
            new BodyPart { Id = 6, Name = "三頭", TrainingDaysPerWeek = 2 },
            new BodyPart { Id = 7, Name = "腹", TrainingDaysPerWeek = 1 }
        );

        // Seed GlobalWeekProfiles
        modelBuilder.Entity<GlobalWeekProfile>().HasData(
            // AMRAP Tracked
            new GlobalWeekProfile { Id = 1, IsAmrapTracked = true, WeekNumber = 1, TargetRpe = 7.0, TargetSets = 3 },
            new GlobalWeekProfile { Id = 2, IsAmrapTracked = true, WeekNumber = 2, TargetRpe = 7.5, TargetSets = 3 },
            new GlobalWeekProfile { Id = 3, IsAmrapTracked = true, WeekNumber = 3, TargetRpe = 8.0, TargetSets = 3 },
            new GlobalWeekProfile { Id = 4, IsAmrapTracked = true, WeekNumber = 4, TargetRpe = 9.0, TargetSets = 3 },
            new GlobalWeekProfile { Id = 5, IsAmrapTracked = true, WeekNumber = 5, TargetRpe = 9.5, TargetSets = 2 },
            new GlobalWeekProfile { Id = 6, IsAmrapTracked = true, WeekNumber = 6, TargetRpe = 7.0, TargetSets = 2 },

            // Non-AMRAP Tracked
            new GlobalWeekProfile { Id = 7, IsAmrapTracked = false, WeekNumber = 1, TargetRpe = 7.0, TargetSets = 3 },
            new GlobalWeekProfile { Id = 8, IsAmrapTracked = false, WeekNumber = 2, TargetRpe = 7.5, TargetSets = 4 },
            new GlobalWeekProfile { Id = 9, IsAmrapTracked = false, WeekNumber = 3, TargetRpe = 8.0, TargetSets = 5 },
            new GlobalWeekProfile { Id = 10, IsAmrapTracked = false, WeekNumber = 4, TargetRpe = 8.0, TargetSets = 3 },
            new GlobalWeekProfile { Id = 11, IsAmrapTracked = false, WeekNumber = 5, TargetRpe = 7.5, TargetSets = 5 },
            new GlobalWeekProfile { Id = 12, IsAmrapTracked = false, WeekNumber = 6, TargetRpe = 7.5, TargetSets = 2 }
        );

        // Seed Exercises
        var exercises = new List<Exercise>
        {
            new Exercise { Id=1, Name="ベンチプレス", BodyPartId=1, BaseTargetRpe=7.0, TargetRepRange="4〜6", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=true, ReferencePercent1RM=0.786, BaseWeight=175.0 },
            new Exercise { Id=2, Name="スクワット", BodyPartId=4, BaseTargetRpe=7.0, TargetRepRange="4〜6", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=true, ReferencePercent1RM=0.786, BaseWeight=220.0 },
            new Exercise { Id=3, Name="ミリタリープレス", BodyPartId=3, BaseTargetRpe=7.5, TargetRepRange="5〜7", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=true, ReferencePercent1RM=0.774, BaseWeight=100.0 },
            new Exercise { Id=4, Name="ルーマニアンデッドリフト", BodyPartId=4, BaseTargetRpe=7.5, TargetRepRange="5〜7", DefaultSetCount=2, WeightIncrement=2.5, IsAmrapTracked=true, ReferencePercent1RM=0.0, BaseWeight=150.0 },
            new Exercise { Id=5, Name="ハイバースクワット", BodyPartId=4, BaseTargetRpe=7.5, TargetRepRange="5〜7", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=true, ReferencePercent1RM=0.0, BaseWeight=190.0 },
            new Exercise { Id=6, Name="インクラインダンベルプレス", BodyPartId=1, BaseTargetRpe=8.0, TargetRepRange="8〜12", DefaultSetCount=3, WeightIncrement=2.0, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=7, Name="ベントオーバーロウ", BodyPartId=2, BaseTargetRpe=7.5, TargetRepRange="5〜7", DefaultSetCount=4, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=110.0 },
            new Exercise { Id=8, Name="ラットプルダウン", BodyPartId=2, BaseTargetRpe=8.0, TargetRepRange="8〜12", DefaultSetCount=4, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=110.0 },
            new Exercise { Id=9, Name="サイドレイズ", BodyPartId=3, BaseTargetRpe=8.5, TargetRepRange="12〜20", DefaultSetCount=3, WeightIncrement=1.0, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=10, Name="ケーブルフライ", BodyPartId=1, BaseTargetRpe=8.5, TargetRepRange="10〜15", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=11, Name="ライイングトライセプスEX", BodyPartId=6, BaseTargetRpe=8.5, TargetRepRange="10〜15", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=12, Name="チェストサポーテッドロウ", BodyPartId=2, BaseTargetRpe=8.0, TargetRepRange="8〜12", DefaultSetCount=5, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=13, Name="レッグカール", BodyPartId=4, BaseTargetRpe=8.5, TargetRepRange="10〜15", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=14, Name="インクラインダンベルカール", BodyPartId=5, BaseTargetRpe=8.5, TargetRepRange="10〜15", DefaultSetCount=4, WeightIncrement=1.0, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=15, Name="レッグエクステンション", BodyPartId=4, BaseTargetRpe=9.0, TargetRepRange="10〜15", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=16, Name="インクラインベンチプレス", BodyPartId=1, BaseTargetRpe=7.5, TargetRepRange="5〜7", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=true, ReferencePercent1RM=0.0, BaseWeight=0 },
            new Exercise { Id=17, Name="マシンチェストプレス", BodyPartId=1, BaseTargetRpe=8.0, TargetRepRange="8〜12", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=18, Name="リアデルトフライ", BodyPartId=3, BaseTargetRpe=8.5, TargetRepRange="12〜20", DefaultSetCount=3, WeightIncrement=1.0, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=19, Name="ケーブルプレスダウン", BodyPartId=6, BaseTargetRpe=8.5, TargetRepRange="10〜15", DefaultSetCount=3, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=20, Name="ブルガリアンスクワット", BodyPartId=4, BaseTargetRpe=8.0, TargetRepRange="6〜8", DefaultSetCount=2, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=80.0 },
            new Exercise { Id=21, Name="ワンハンドラットプル", BodyPartId=2, BaseTargetRpe=8.0, TargetRepRange="10〜15", DefaultSetCount=4, WeightIncrement=2.5, IsAmrapTracked=false, BaseWeight=0 },
            new Exercise { Id=22, Name="ダンベルカール", BodyPartId=5, BaseTargetRpe=9.0, TargetRepRange="10〜15", DefaultSetCount=4, WeightIncrement=1.0, IsAmrapTracked=false, BaseWeight=0 }
        };

        modelBuilder.Entity<Exercise>().HasData(exercises);

        // Generate and seed WeekProfiles dynamically
        var weekProfiles = new List<WeekProfile>();
        int wpId = 1;

        foreach (var ex in exercises)
        {
            if (ex.IsAmrapTracked)
            {
                double amrapRpe = ex.TargetRepRange == "4〜6" ? 9.5 : 9.0;
                
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 1, TargetRpe = ex.BaseTargetRpe, TargetSets = ex.DefaultSetCount, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 2, TargetRpe = ex.BaseTargetRpe + 0.5, TargetSets = ex.DefaultSetCount, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 3, TargetRpe = ex.BaseTargetRpe + 1.0, TargetSets = ex.DefaultSetCount, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 4, TargetRpe = 9.0, TargetSets = ex.DefaultSetCount, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 5, TargetRpe = amrapRpe, TargetSets = 2, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 6, TargetRpe = 7.0, TargetSets = 2, RepRange = "" });
            }
            else
            {
                int w235Sets = Math.Min(5, ex.DefaultSetCount + 2);
                
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 1, TargetRpe = ex.BaseTargetRpe, TargetSets = ex.DefaultSetCount, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 2, TargetRpe = ex.BaseTargetRpe + 0.5, TargetSets = w235Sets, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 3, TargetRpe = ex.BaseTargetRpe + 1.0, TargetSets = w235Sets, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 4, TargetRpe = ex.BaseTargetRpe + 1.0, TargetSets = ex.DefaultSetCount, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 5, TargetRpe = ex.BaseTargetRpe + 0.5, TargetSets = w235Sets, RepRange = "" });
                weekProfiles.Add(new WeekProfile { Id = wpId++, ExerciseId = ex.Id, WeekNumber = 6, TargetRpe = 7.5, TargetSets = 2, RepRange = "" });
            }
        }

        modelBuilder.Entity<WeekProfile>().HasData(weekProfiles);
    }
}
