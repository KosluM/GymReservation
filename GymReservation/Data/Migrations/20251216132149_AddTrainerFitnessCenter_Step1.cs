using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymReservation.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainerFitnessCenter_Step1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Önce NULL kabul eden kolon ekle (default 0 basma!)
            migrationBuilder.AddColumn<int>(
                name: "FitnessCenterId",
                table: "Trainers",
                type: "int",
                nullable: true);

            // 2) Mevcut trainer kayıtlarını geçerli bir salon ile doldur
            // FitnessCenters tablosundaki ilk Id alınır ve NULL olanlara atanır.
            migrationBuilder.Sql(@"
DECLARE @fcId INT = (SELECT TOP 1 Id FROM FitnessCenters ORDER BY Id);
IF (@fcId IS NOT NULL)
BEGIN
    UPDATE Trainers
    SET FitnessCenterId = @fcId
    WHERE FitnessCenterId IS NULL;
END
");

            // 3) Kolonu NOT NULL yap
            migrationBuilder.AlterColumn<int>(
                name: "FitnessCenterId",
                table: "Trainers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // 4) Index
            migrationBuilder.CreateIndex(
                name: "IX_Trainers_FitnessCenterId",
                table: "Trainers",
                column: "FitnessCenterId");

            // 5) FK (CASCADE kapalı)
            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_FitnessCenters_FitnessCenterId",
                table: "Trainers",
                column: "FitnessCenterId",
                principalTable: "FitnessCenters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_FitnessCenters_FitnessCenterId",
                table: "Trainers");

            migrationBuilder.DropIndex(
                name: "IX_Trainers_FitnessCenterId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "FitnessCenterId",
                table: "Trainers");
        }
    }
}
