using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WorkoutTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalWeekProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCustomWeekProfile",
                table: "Exercises",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GlobalWeekProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    IsAmrapTracked = table.Column<bool>(type: "INTEGER", nullable: false),
                    WeekNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    TargetRpe = table.Column<double>(type: "REAL", nullable: false),
                    TargetSets = table.Column<int>(type: "INTEGER", nullable: false),
                    RepRange = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalWeekProfiles", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 1,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 2,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 3,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 4,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 5,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 6,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 7,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 8,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 9,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 10,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 11,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 12,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 13,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 14,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 15,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 16,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 17,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 18,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 19,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 20,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 21,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.UpdateData(
                table: "Exercises",
                keyColumn: "Id",
                keyValue: 22,
                column: "IsCustomWeekProfile",
                value: false);

            migrationBuilder.InsertData(
                table: "GlobalWeekProfiles",
                columns: new[] { "Id", "IsAmrapTracked", "RepRange", "TargetRpe", "TargetSets", "WeekNumber" },
                values: new object[,]
                {
                    { 1, true, "", 7.0, 3, 1 },
                    { 2, true, "", 7.5, 3, 2 },
                    { 3, true, "", 8.0, 3, 3 },
                    { 4, true, "", 9.0, 3, 4 },
                    { 5, true, "", 9.5, 2, 5 },
                    { 6, true, "", 7.0, 2, 6 },
                    { 7, false, "", 7.0, 3, 1 },
                    { 8, false, "", 7.5, 4, 2 },
                    { 9, false, "", 8.0, 5, 3 },
                    { 10, false, "", 8.0, 3, 4 },
                    { 11, false, "", 7.5, 5, 5 },
                    { 12, false, "", 7.5, 2, 6 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalWeekProfiles");

            migrationBuilder.DropColumn(
                name: "IsCustomWeekProfile",
                table: "Exercises");
        }
    }
}
