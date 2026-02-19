using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagmentDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionBasedPlans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSessionBased",
                table: "Planes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "SessionCount",
                table: "Planes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SessionsRemaining",
                table: "MemberShips",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSessionBased",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "SessionCount",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "SessionsRemaining",
                table: "MemberShips");
        }
    }
}
