using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagmentDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateStoreAccountingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmount",
                table: "Sales",
                newName: "NetTotal");

            migrationBuilder.RenameColumn(
                name: "DiscountAmount",
                table: "Sales",
                newName: "TotalDiscount");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalBeforeDiscount",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "ItemProfit",
                table: "SaleItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetTotal",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ItemProfit",
                table: "SaleItems");

            migrationBuilder.RenameColumn(
                name: "TotalDiscount",
                table: "Sales",
                newName: "TotalAmount");

            migrationBuilder.RenameColumn(
                name: "TotalBeforeDiscount",
                table: "Sales",
                newName: "DiscountAmount");
        }
    }
}
