using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorkoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class Add6WeekCycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyParts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TrainingDaysPerWeek = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyParts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TrainingSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Memo = table.Column<string>(type: "TEXT", nullable: false),
                    CycleNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    WeekInCycle = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainingSessions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Exercises",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    BodyPartId = table.Column<int>(type: "INTEGER", nullable: false),
                    BaseTargetRpe = table.Column<double>(type: "REAL", nullable: false),
                    TargetRepRange = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultSetCount = table.Column<int>(type: "INTEGER", nullable: false),
                    WeightIncrement = table.Column<double>(type: "REAL", nullable: false),
                    IsAmrapTracked = table.Column<bool>(type: "INTEGER", nullable: false),
                    AmrapWeight = table.Column<double>(type: "REAL", nullable: false),
                    AmrapRep = table.Column<int>(type: "INTEGER", nullable: false),
                    AmrapE1RM = table.Column<double>(type: "REAL", nullable: false),
                    ReferencePercent1RM = table.Column<double>(type: "REAL", nullable: false),
                    BaseWeight = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exercises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Exercises_BodyParts_BodyPartId",
                        column: x => x.BodyPartId,
                        principalTable: "BodyParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExerciseRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrainingSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    SetNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Weight = table.Column<double>(type: "REAL", nullable: false),
                    Rep = table.Column<int>(type: "INTEGER", nullable: false),
                    Rpe = table.Column<double>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExerciseRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExerciseRecords_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExerciseRecords_TrainingSessions_TrainingSessionId",
                        column: x => x.TrainingSessionId,
                        principalTable: "TrainingSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeekProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ExerciseId = table.Column<int>(type: "INTEGER", nullable: false),
                    WeekNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetRpe = table.Column<double>(type: "REAL", nullable: false),
                    TargetSets = table.Column<int>(type: "INTEGER", nullable: false),
                    RepRange = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekProfiles_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "BodyParts",
                columns: new[] { "Id", "Name", "TrainingDaysPerWeek" },
                values: new object[,]
                {
                    { 1, "胸", 2 },
                    { 2, "背中", 2 },
                    { 3, "肩", 2 },
                    { 4, "脚", 2 },
                    { 5, "二頭", 2 },
                    { 6, "三頭", 2 },
                    { 7, "腹", 1 }
                });

            migrationBuilder.InsertData(
                table: "Exercises",
                columns: new[] { "Id", "AmrapE1RM", "AmrapRep", "AmrapWeight", "BaseTargetRpe", "BaseWeight", "BodyPartId", "DefaultSetCount", "IsAmrapTracked", "Name", "ReferencePercent1RM", "TargetRepRange", "WeightIncrement" },
                values: new object[,]
                {
                    { 1, 0.0, 0, 0.0, 7.0, 175.0, 1, 3, true, "ベンチプレス", 0.78600000000000003, "4〜6", 2.5 },
                    { 2, 0.0, 0, 0.0, 7.0, 220.0, 4, 3, true, "スクワット", 0.78600000000000003, "4〜6", 2.5 },
                    { 3, 0.0, 0, 0.0, 7.5, 100.0, 3, 3, true, "ミリタリープレス", 0.77400000000000002, "5〜7", 2.5 },
                    { 4, 0.0, 0, 0.0, 7.5, 150.0, 4, 2, true, "ルーマニアンデッドリフト", 0.0, "5〜7", 2.5 },
                    { 5, 0.0, 0, 0.0, 7.5, 190.0, 4, 3, true, "ハイバースクワット", 0.0, "5〜7", 2.5 },
                    { 6, 0.0, 0, 0.0, 8.0, 0.0, 1, 3, false, "インクラインダンベルプレス", 0.0, "8〜12", 2.0 },
                    { 7, 0.0, 0, 0.0, 7.5, 110.0, 2, 4, false, "ベントオーバーロウ", 0.0, "5〜7", 2.5 },
                    { 8, 0.0, 0, 0.0, 8.0, 110.0, 2, 4, false, "ラットプルダウン", 0.0, "8〜12", 2.5 },
                    { 9, 0.0, 0, 0.0, 8.5, 0.0, 3, 3, false, "サイドレイズ", 0.0, "12〜20", 1.0 },
                    { 10, 0.0, 0, 0.0, 8.5, 0.0, 1, 3, false, "ケーブルフライ", 0.0, "10〜15", 2.5 },
                    { 11, 0.0, 0, 0.0, 8.5, 0.0, 6, 3, false, "ライイングトライセプスEX", 0.0, "10〜15", 2.5 },
                    { 12, 0.0, 0, 0.0, 8.0, 0.0, 2, 5, false, "チェストサポーテッドロウ", 0.0, "8〜12", 2.5 },
                    { 13, 0.0, 0, 0.0, 8.5, 0.0, 4, 3, false, "レッグカール", 0.0, "10〜15", 2.5 },
                    { 14, 0.0, 0, 0.0, 8.5, 0.0, 5, 4, false, "インクラインダンベルカール", 0.0, "10〜15", 1.0 },
                    { 15, 0.0, 0, 0.0, 9.0, 0.0, 4, 3, false, "レッグエクステンション", 0.0, "10〜15", 2.5 },
                    { 16, 0.0, 0, 0.0, 7.5, 0.0, 1, 3, true, "インクラインベンチプレス", 0.0, "5〜7", 2.5 },
                    { 17, 0.0, 0, 0.0, 8.0, 0.0, 1, 3, false, "マシンチェストプレス", 0.0, "8〜12", 2.5 },
                    { 18, 0.0, 0, 0.0, 8.5, 0.0, 3, 3, false, "リアデルトフライ", 0.0, "12〜20", 1.0 },
                    { 19, 0.0, 0, 0.0, 8.5, 0.0, 6, 3, false, "ケーブルプレスダウン", 0.0, "10〜15", 2.5 },
                    { 20, 0.0, 0, 0.0, 8.0, 80.0, 4, 2, false, "ブルガリアンスクワット", 0.0, "6〜8", 2.5 },
                    { 21, 0.0, 0, 0.0, 8.0, 0.0, 2, 4, false, "ワンハンドラットプル", 0.0, "10〜15", 2.5 },
                    { 22, 0.0, 0, 0.0, 9.0, 0.0, 5, 4, false, "ダンベルカール", 0.0, "10〜15", 1.0 }
                });

            migrationBuilder.InsertData(
                table: "WeekProfiles",
                columns: new[] { "Id", "ExerciseId", "RepRange", "TargetRpe", "TargetSets", "WeekNumber" },
                values: new object[,]
                {
                    { 1, 1, "", 7.0, 3, 1 },
                    { 2, 1, "", 7.5, 3, 2 },
                    { 3, 1, "", 8.0, 3, 3 },
                    { 4, 1, "", 9.0, 3, 4 },
                    { 5, 1, "", 9.5, 2, 5 },
                    { 6, 1, "", 7.0, 2, 6 },
                    { 7, 2, "", 7.0, 3, 1 },
                    { 8, 2, "", 7.5, 3, 2 },
                    { 9, 2, "", 8.0, 3, 3 },
                    { 10, 2, "", 9.0, 3, 4 },
                    { 11, 2, "", 9.5, 2, 5 },
                    { 12, 2, "", 7.0, 2, 6 },
                    { 13, 3, "", 7.5, 3, 1 },
                    { 14, 3, "", 8.0, 3, 2 },
                    { 15, 3, "", 8.5, 3, 3 },
                    { 16, 3, "", 9.0, 3, 4 },
                    { 17, 3, "", 9.0, 2, 5 },
                    { 18, 3, "", 7.0, 2, 6 },
                    { 19, 4, "", 7.5, 2, 1 },
                    { 20, 4, "", 8.0, 2, 2 },
                    { 21, 4, "", 8.5, 2, 3 },
                    { 22, 4, "", 9.0, 2, 4 },
                    { 23, 4, "", 9.0, 2, 5 },
                    { 24, 4, "", 7.0, 2, 6 },
                    { 25, 5, "", 7.5, 3, 1 },
                    { 26, 5, "", 8.0, 3, 2 },
                    { 27, 5, "", 8.5, 3, 3 },
                    { 28, 5, "", 9.0, 3, 4 },
                    { 29, 5, "", 9.0, 2, 5 },
                    { 30, 5, "", 7.0, 2, 6 },
                    { 31, 6, "", 8.0, 3, 1 },
                    { 32, 6, "", 8.5, 5, 2 },
                    { 33, 6, "", 9.0, 5, 3 },
                    { 34, 6, "", 9.0, 3, 4 },
                    { 35, 6, "", 8.5, 5, 5 },
                    { 36, 6, "", 7.5, 2, 6 },
                    { 37, 7, "", 7.5, 4, 1 },
                    { 38, 7, "", 8.0, 5, 2 },
                    { 39, 7, "", 8.5, 5, 3 },
                    { 40, 7, "", 8.5, 4, 4 },
                    { 41, 7, "", 8.0, 5, 5 },
                    { 42, 7, "", 7.5, 2, 6 },
                    { 43, 8, "", 8.0, 4, 1 },
                    { 44, 8, "", 8.5, 5, 2 },
                    { 45, 8, "", 9.0, 5, 3 },
                    { 46, 8, "", 9.0, 4, 4 },
                    { 47, 8, "", 8.5, 5, 5 },
                    { 48, 8, "", 7.5, 2, 6 },
                    { 49, 9, "", 8.5, 3, 1 },
                    { 50, 9, "", 9.0, 5, 2 },
                    { 51, 9, "", 9.5, 5, 3 },
                    { 52, 9, "", 9.5, 3, 4 },
                    { 53, 9, "", 9.0, 5, 5 },
                    { 54, 9, "", 7.5, 2, 6 },
                    { 55, 10, "", 8.5, 3, 1 },
                    { 56, 10, "", 9.0, 5, 2 },
                    { 57, 10, "", 9.5, 5, 3 },
                    { 58, 10, "", 9.5, 3, 4 },
                    { 59, 10, "", 9.0, 5, 5 },
                    { 60, 10, "", 7.5, 2, 6 },
                    { 61, 11, "", 8.5, 3, 1 },
                    { 62, 11, "", 9.0, 5, 2 },
                    { 63, 11, "", 9.5, 5, 3 },
                    { 64, 11, "", 9.5, 3, 4 },
                    { 65, 11, "", 9.0, 5, 5 },
                    { 66, 11, "", 7.5, 2, 6 },
                    { 67, 12, "", 8.0, 5, 1 },
                    { 68, 12, "", 8.5, 5, 2 },
                    { 69, 12, "", 9.0, 5, 3 },
                    { 70, 12, "", 9.0, 5, 4 },
                    { 71, 12, "", 8.5, 5, 5 },
                    { 72, 12, "", 7.5, 2, 6 },
                    { 73, 13, "", 8.5, 3, 1 },
                    { 74, 13, "", 9.0, 5, 2 },
                    { 75, 13, "", 9.5, 5, 3 },
                    { 76, 13, "", 9.5, 3, 4 },
                    { 77, 13, "", 9.0, 5, 5 },
                    { 78, 13, "", 7.5, 2, 6 },
                    { 79, 14, "", 8.5, 4, 1 },
                    { 80, 14, "", 9.0, 5, 2 },
                    { 81, 14, "", 9.5, 5, 3 },
                    { 82, 14, "", 9.5, 4, 4 },
                    { 83, 14, "", 9.0, 5, 5 },
                    { 84, 14, "", 7.5, 2, 6 },
                    { 85, 15, "", 9.0, 3, 1 },
                    { 86, 15, "", 9.5, 5, 2 },
                    { 87, 15, "", 10.0, 5, 3 },
                    { 88, 15, "", 10.0, 3, 4 },
                    { 89, 15, "", 9.5, 5, 5 },
                    { 90, 15, "", 7.5, 2, 6 },
                    { 91, 16, "", 7.5, 3, 1 },
                    { 92, 16, "", 8.0, 3, 2 },
                    { 93, 16, "", 8.5, 3, 3 },
                    { 94, 16, "", 9.0, 3, 4 },
                    { 95, 16, "", 9.0, 2, 5 },
                    { 96, 16, "", 7.0, 2, 6 },
                    { 97, 17, "", 8.0, 3, 1 },
                    { 98, 17, "", 8.5, 5, 2 },
                    { 99, 17, "", 9.0, 5, 3 },
                    { 100, 17, "", 9.0, 3, 4 },
                    { 101, 17, "", 8.5, 5, 5 },
                    { 102, 17, "", 7.5, 2, 6 },
                    { 103, 18, "", 8.5, 3, 1 },
                    { 104, 18, "", 9.0, 5, 2 },
                    { 105, 18, "", 9.5, 5, 3 },
                    { 106, 18, "", 9.5, 3, 4 },
                    { 107, 18, "", 9.0, 5, 5 },
                    { 108, 18, "", 7.5, 2, 6 },
                    { 109, 19, "", 8.5, 3, 1 },
                    { 110, 19, "", 9.0, 5, 2 },
                    { 111, 19, "", 9.5, 5, 3 },
                    { 112, 19, "", 9.5, 3, 4 },
                    { 113, 19, "", 9.0, 5, 5 },
                    { 114, 19, "", 7.5, 2, 6 },
                    { 115, 20, "", 8.0, 2, 1 },
                    { 116, 20, "", 8.5, 4, 2 },
                    { 117, 20, "", 9.0, 4, 3 },
                    { 118, 20, "", 9.0, 2, 4 },
                    { 119, 20, "", 8.5, 4, 5 },
                    { 120, 20, "", 7.5, 2, 6 },
                    { 121, 21, "", 8.0, 4, 1 },
                    { 122, 21, "", 8.5, 5, 2 },
                    { 123, 21, "", 9.0, 5, 3 },
                    { 124, 21, "", 9.0, 4, 4 },
                    { 125, 21, "", 8.5, 5, 5 },
                    { 126, 21, "", 7.5, 2, 6 },
                    { 127, 22, "", 9.0, 4, 1 },
                    { 128, 22, "", 9.5, 5, 2 },
                    { 129, 22, "", 10.0, 5, 3 },
                    { 130, 22, "", 10.0, 4, 4 },
                    { 131, 22, "", 9.5, 5, 5 },
                    { 132, 22, "", 7.5, 2, 6 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseRecords_ExerciseId",
                table: "ExerciseRecords",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExerciseRecords_TrainingSessionId",
                table: "ExerciseRecords",
                column: "TrainingSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Exercises_BodyPartId",
                table: "Exercises",
                column: "BodyPartId");

            migrationBuilder.CreateIndex(
                name: "IX_WeekProfiles_ExerciseId",
                table: "WeekProfiles",
                column: "ExerciseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExerciseRecords");

            migrationBuilder.DropTable(
                name: "WeekProfiles");

            migrationBuilder.DropTable(
                name: "TrainingSessions");

            migrationBuilder.DropTable(
                name: "Exercises");

            migrationBuilder.DropTable(
                name: "BodyParts");
        }
    }
}
