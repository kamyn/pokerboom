using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerBoom.Server.Migrations
{
    public partial class add_table_players : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13fd14c9-99cb-46d2-b390-8dbeaffcc01e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4914fd68-f140-4a65-a578-e0281d0b3261");

            migrationBuilder.AddColumn<int>(
                name: "Players",
                table: "Tables",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8ed5fe4b-b196-4926-a975-9cbaa87855c0", "4efbd066-46f5-46c5-9453-154f39711e00", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "aed4669f-65c7-40bc-8c46-9d73d022b660", "fdc936e1-442f-465f-8e81-75218d8234d9", "User", "USER" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ed5fe4b-b196-4926-a975-9cbaa87855c0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "aed4669f-65c7-40bc-8c46-9d73d022b660");

            migrationBuilder.DropColumn(
                name: "Players",
                table: "Tables");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "13fd14c9-99cb-46d2-b390-8dbeaffcc01e", "b6d1d7be-d3e8-485f-a1e9-183cfd233ecc", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4914fd68-f140-4a65-a578-e0281d0b3261", "9a3e8b31-ea29-41fc-a25e-21b6db5c26c5", "Administrator", "ADMINISTRATOR" });
        }
    }
}
