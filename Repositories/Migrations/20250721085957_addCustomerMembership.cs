using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class addCustomerMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Membership_Customers_CustomerId",
                table: "Membership");

            migrationBuilder.DropForeignKey(
                name: "FK_Rating_OrderDetails_OrderDetailId",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rating",
                table: "Rating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Membership",
                table: "Membership");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Membership");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Membership");

            migrationBuilder.RenameTable(
                name: "Rating",
                newName: "Ratings");

            migrationBuilder.RenameTable(
                name: "Membership",
                newName: "Memberships");

            migrationBuilder.RenameIndex(
                name: "IX_Rating_OrderDetailId",
                table: "Ratings",
                newName: "IX_Ratings_OrderDetailId");

            migrationBuilder.RenameColumn(
                name: "imgURL",
                table: "Memberships",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "MemberShip",
                table: "Memberships",
                newName: "DurationInDays");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Memberships",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Memberships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "Memberships",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Memberships",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Memberships",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Memberships",
                table: "Memberships",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CustomerMemberships",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MembershipId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DurationInDays = table.Column<int>(type: "int", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerMemberships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerMemberships_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CustomerMemberships_Memberships_MembershipId",
                        column: x => x.MembershipId,
                        principalTable: "Memberships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMemberships_CustomerId",
                table: "CustomerMemberships",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerMemberships_MembershipId",
                table: "CustomerMemberships",
                column: "MembershipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_OrderDetails_OrderDetailId",
                table: "Ratings",
                column: "OrderDetailId",
                principalTable: "OrderDetails",
                principalColumn: "OrderDetailId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_OrderDetails_OrderDetailId",
                table: "Ratings");

            migrationBuilder.DropTable(
                name: "CustomerMemberships");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Memberships",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Memberships");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "Memberships");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "Rating");

            migrationBuilder.RenameTable(
                name: "Memberships",
                newName: "Membership");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_OrderDetailId",
                table: "Rating",
                newName: "IX_Rating_OrderDetailId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Membership",
                newName: "imgURL");

            migrationBuilder.RenameColumn(
                name: "DurationInDays",
                table: "Membership",
                newName: "MemberShip");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Membership",
                newName: "CustomerId");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Membership",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Membership",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rating",
                table: "Rating",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Membership",
                table: "Membership",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Membership_Customers_CustomerId",
                table: "Membership",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_OrderDetails_OrderDetailId",
                table: "Rating",
                column: "OrderDetailId",
                principalTable: "OrderDetails",
                principalColumn: "OrderDetailId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
