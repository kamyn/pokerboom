using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerBoom.Server.Migrations
{
    public partial class roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2291a7b7-18be-4a45-8a68-b57de4ccfdb0", "4ff8d63e-b3dd-490e-80e2-c84abf59d0db", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "3efc0c3f-7cf6-4816-9e3c-16e8d063d020", "68c7cb90-c435-49a9-abcf-553574e64203", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2291a7b7-18be-4a45-8a68-b57de4ccfdb0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3efc0c3f-7cf6-4816-9e3c-16e8d063d020");
        }
    }
}
