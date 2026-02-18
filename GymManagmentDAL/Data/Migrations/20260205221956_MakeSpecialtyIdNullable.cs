using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagmentDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class MakeSpecialtyIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_TrainerSpecialties_SpecialtyId",
                table: "Trainers");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialtyId",
                table: "Trainers",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_TrainerSpecialties_SpecialtyId",
                table: "Trainers",
                column: "SpecialtyId",
                principalTable: "TrainerSpecialties",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trainers_TrainerSpecialties_SpecialtyId",
                table: "Trainers");

            migrationBuilder.AlterColumn<int>(
                name: "SpecialtyId",
                table: "Trainers",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trainers_TrainerSpecialties_SpecialtyId",
                table: "Trainers",
                column: "SpecialtyId",
                principalTable: "TrainerSpecialties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
