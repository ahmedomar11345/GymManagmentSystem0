using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagmentDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWalkInBookingAndSessionPrice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SessionPrice",
                table: "GymSettings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "WalkInBookings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuestName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuestPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MemberId = table.Column<int>(type: "int", nullable: true),
                    SessionCount = table.Column<int>(type: "int", nullable: false),
                    PricePerSession = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SessionsUsed = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalkInBookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalkInBookings_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WalkInSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalkInBookingId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    IsAttended = table.Column<bool>(type: "bit", nullable: false),
                    AttendedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalkInSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalkInSessions_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalkInSessions_WalkInBookings_WalkInBookingId",
                        column: x => x.WalkInBookingId,
                        principalTable: "WalkInBookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalkInBookings_MemberId",
                table: "WalkInBookings",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_WalkInSessions_SessionId",
                table: "WalkInSessions",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WalkInSessions_WalkInBookingId",
                table: "WalkInSessions",
                column: "WalkInBookingId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalkInSessions");

            migrationBuilder.DropTable(
                name: "WalkInBookings");

            migrationBuilder.DropColumn(
                name: "SessionPrice",
                table: "GymSettings");
        }
    }
}
