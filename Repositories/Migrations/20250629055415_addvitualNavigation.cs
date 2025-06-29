using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositories.Migrations
{
    /// <inheritdoc />
    public partial class addvitualNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Staffs_StaffProfileId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StaffProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StaffProfileId",
                table: "Users");

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Users_Id",
                table: "Staffs",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Users_Id",
                table: "Staffs");

            migrationBuilder.AddColumn<Guid>(
                name: "StaffProfileId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_StaffProfileId",
                table: "Users",
                column: "StaffProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Staffs_StaffProfileId",
                table: "Users",
                column: "StaffProfileId",
                principalTable: "Staffs",
                principalColumn: "Id");
        }
    }
}
