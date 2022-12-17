using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerBoom.Server.Migrations
{
    public partial class roles1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2291a7b7-18be-4a45-8a68-b57de4ccfdb0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3efc0c3f-7cf6-4816-9e3c-16e8d063d020");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "012484cc-4423-4777-9988-90732c4a5a72", "b8c201e3-ffc3-4279-a312-368bcab8b19d", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2d220f48-d2ef-424c-8f38-9b6f53cdca9c", "0d52eeff-a2d5-4585-a300-2bfccedf505a", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "012484cc-4423-4777-9988-90732c4a5a72");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2d220f48-d2ef-424c-8f38-9b6f53cdca9c");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2291a7b7-18be-4a45-8a68-b57de4ccfdb0", "4ff8d63e-b3dd-490e-80e2-c84abf59d0db", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3efc0c3f-7cf6-4816-9e3c-16e8d063d020", "68c7cb90-c435-49a9-abcf-553574e64203", "User", "USER" });
        }
    }
}
