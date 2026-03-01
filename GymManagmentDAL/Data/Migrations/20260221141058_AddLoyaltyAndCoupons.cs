using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymManagmentDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltyAndCoupons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsVoided",
                table: "Sales");

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "WalkInSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WalkInSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "WalkInSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "WalkInSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "WalkInBookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "WalkInBookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "WalkInBookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "WalkInBookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "TrainerSpecialties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TrainerSpecialties",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "TrainerSpecialties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "TrainerSpecialties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Trainers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "TrainerAttendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "TrainerAttendances",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "TrainerAttendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "TrainerAttendances",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Suppliers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "StoreProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StoreProducts",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "StoreProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "StoreProducts",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "StoreProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "StoreProductImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StoreProductImages",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "StoreProductImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "StoreProductImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "StoreCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StoreCategories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "StoreCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "StoreCategories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "StockAdjustments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "StockAdjustments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "StockAdjustments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "StockAdjustments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CouponCode",
                table: "Sales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Sales",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberNameSnapshot",
                table: "Sales",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MemberStatusSnapshot",
                table: "Sales",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PointsEarned",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PointsRedeemedValue",
                table: "Sales",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Sales",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "SaleItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SaleItems",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "SaleItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RefundedQuantity",
                table: "SaleItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StoreProductVariantId",
                table: "SaleItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "SaleItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Planes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Planes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Planes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Planes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "MemberShips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MemberShips",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "MemberShips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "MemberShips",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "MemberSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "MemberSessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "MemberSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "MemberSessions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Members",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LoyaltyPoints",
                table: "Members",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Members",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "HealthProgresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "HealthProgresses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "HealthProgresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "HealthProgresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "GymSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "GymSettings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "GymSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "GymSettings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Expenses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "CheckIns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CheckIns",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "CheckIns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "CheckIns",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Categories",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "AuditLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByUserId",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Coupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: false),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    MinimumAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoreProductVariants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    StoreProductId = table.Column<int>(type: "int", nullable: false),
                    VariantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PriceAdjustment = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreProductVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StoreProductVariants_StoreProducts_StoreProductId",
                        column: x => x.StoreProductId,
                        principalTable: "StoreProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "TrainerSpecialties",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedByUserId", "DeletedAt", "DeletedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TrainerSpecialties",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedByUserId", "DeletedAt", "DeletedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TrainerSpecialties",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedByUserId", "DeletedAt", "DeletedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TrainerSpecialties",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedByUserId", "DeletedAt", "DeletedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TrainerSpecialties",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedByUserId", "DeletedAt", "DeletedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null, null, null });

            migrationBuilder.UpdateData(
                table: "TrainerSpecialties",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "CreatedByUserId", "DeletedAt", "DeletedByUserId", "UpdatedByUserId" },
                values: new object[] { null, null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_Barcode",
                table: "StoreProducts",
                column: "Barcode");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProducts_SKU",
                table: "StoreProducts",
                column: "SKU",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CouponCode",
                table: "Sales",
                column: "CouponCode");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_SaleDate",
                table: "Sales",
                column: "SaleDate");

            migrationBuilder.CreateIndex(
                name: "IX_SaleItems_StoreProductVariantId",
                table: "SaleItems",
                column: "StoreProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Coupons_Code",
                table: "Coupons",
                column: "Code",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductVariants_SKU",
                table: "StoreProductVariants",
                column: "SKU",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_StoreProductVariants_StoreProductId",
                table: "StoreProductVariants",
                column: "StoreProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_SaleItems_StoreProductVariants_StoreProductVariantId",
                table: "SaleItems",
                column: "StoreProductVariantId",
                principalTable: "StoreProductVariants",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SaleItems_StoreProductVariants_StoreProductVariantId",
                table: "SaleItems");

            migrationBuilder.DropTable(
                name: "Coupons");

            migrationBuilder.DropTable(
                name: "StoreProductVariants");

            migrationBuilder.DropIndex(
                name: "IX_StoreProducts_Barcode",
                table: "StoreProducts");

            migrationBuilder.DropIndex(
                name: "IX_StoreProducts_SKU",
                table: "StoreProducts");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CouponCode",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_Sales_SaleDate",
                table: "Sales");

            migrationBuilder.DropIndex(
                name: "IX_SaleItems_StoreProductVariantId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "WalkInSessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WalkInSessions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "WalkInSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "WalkInSessions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "WalkInBookings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "WalkInBookings");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "WalkInBookings");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "WalkInBookings");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TrainerSpecialties");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TrainerSpecialties");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "TrainerSpecialties");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "TrainerSpecialties");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TrainerAttendances");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "TrainerAttendances");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "TrainerAttendances");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "TrainerAttendances");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "StoreProducts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "StoreProductImages");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StoreProductImages");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "StoreProductImages");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "StoreProductImages");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "StoreCategories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StoreCategories");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "StoreCategories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "StoreCategories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "StockAdjustments");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "CouponCode",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "MemberNameSnapshot",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "MemberStatusSnapshot",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PointsEarned",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PointsRedeemedValue",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "RefundedQuantity",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "StoreProductVariantId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "SaleItems");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Planes");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MemberShips");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MemberShips");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "MemberShips");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MemberShips");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "MemberSessions");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "MemberSessions");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "MemberSessions");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "MemberSessions");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "LoyaltyPoints",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "HealthProgresses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "HealthProgresses");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "HealthProgresses");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "HealthProgresses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "GymSettings");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "GymSettings");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "GymSettings");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "GymSettings");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "DeletedByUserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "UpdatedByUserId",
                table: "AuditLogs");

            migrationBuilder.AddColumn<bool>(
                name: "IsVoided",
                table: "Sales",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
