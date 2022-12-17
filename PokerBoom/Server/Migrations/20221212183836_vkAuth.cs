using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerBoom.Server.Migrations
{
    public partial class vkAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "012484cc-4423-4777-9988-90732c4a5a72");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d220f48-d2ef-424c-8f38-9b6f53cdca9c");

            migrationBuilder.AddColumn<string>(
                name: "VkId",
                table: "AspNetUsers",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "730e4eaf-bd64-43e6-8e5b-6082159b052c", "a80d9679-363f-496d-88f2-41968efcfa9c", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "cc4114a3-8a14-489e-a6ae-1842d74fbceb", "7edb1d2b-daab-4a44-b883-799d7b93268f", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "730e4eaf-bd64-43e6-8e5b-6082159b052c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc4114a3-8a14-489e-a6ae-1842d74fbceb");

            migrationBuilder.DropColumn(
                name: "VkId",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "012484cc-4423-4777-9988-90732c4a5a72", "b8c201e3-ffc3-4279-a312-368bcab8b19d", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2d220f48-d2ef-424c-8f38-9b6f53cdca9c", "0d52eeff-a2d5-4585-a300-2bfccedf505a", "User", "USER" });
        }
    }
}
