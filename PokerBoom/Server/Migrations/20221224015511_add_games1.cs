using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokerBoom.Server.Migrations
{
    public partial class add_games1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_AspNetUsers_UserId1",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_UserId1",
                table: "Players");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1c8fa601-9ef0-48fc-9b13-3778f239105a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "383ec349-6aaa-48e6-b63a-cabb1d337326");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Players");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Players",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "13fd14c9-99cb-46d2-b390-8dbeaffcc01e", "b6d1d7be-d3e8-485f-a1e9-183cfd233ecc", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4914fd68-f140-4a65-a578-e0281d0b3261", "9a3e8b31-ea29-41fc-a25e-21b6db5c26c5", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "Tables",
                columns: new[] { "Id", "Name", "SmallBlind" },
                values: new object[] { 1, "стол #1", 10 });

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_AspNetUsers_UserId",
                table: "Players",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_AspNetUsers_UserId",
                table: "Players");

            migrationBuilder.DropIndex(
                name: "IX_Players_UserId",
                table: "Players");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "13fd14c9-99cb-46d2-b390-8dbeaffcc01e");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4914fd68-f140-4a65-a578-e0281d0b3261");

            migrationBuilder.DeleteData(
                table: "Tables",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Players",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Players",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "1c8fa601-9ef0-48fc-9b13-3778f239105a", "d0aa2a3d-9506-4bb3-84d9-9818727f3d68", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "383ec349-6aaa-48e6-b63a-cabb1d337326", "2a1e601f-091e-4faf-aa4a-ac1b1cf0ea37", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId1",
                table: "Players",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_AspNetUsers_UserId1",
                table: "Players",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
