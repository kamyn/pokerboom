using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerBoom.Server.Migrations
{
    public partial class currency_add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "730e4eaf-bd64-43e6-8e5b-6082159b052c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc4114a3-8a14-489e-a6ae-1842d74fbceb");

            migrationBuilder.AddColumn<int>(
                name: "Currency",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "c9170df6-9c4f-4a48-af18-e163683c43aa", "6dccac1d-8d50-4f4f-9adc-8924b4387b1f", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "faede6e5-850f-4d64-9988-9acab258170d", "83bd1611-bc70-4102-92f9-2351e18c30f2", "Administrator", "ADMINISTRATOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "c9170df6-9c4f-4a48-af18-e163683c43aa");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "faede6e5-850f-4d64-9988-9acab258170d");

            migrationBuilder.DropColumn(
                name: "Currency",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "730e4eaf-bd64-43e6-8e5b-6082159b052c", "a80d9679-363f-496d-88f2-41968efcfa9c", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "cc4114a3-8a14-489e-a6ae-1842d74fbceb", "7edb1d2b-daab-4a44-b883-799d7b93268f", "Administrator", "ADMINISTRATOR" });
        }
    }
}
