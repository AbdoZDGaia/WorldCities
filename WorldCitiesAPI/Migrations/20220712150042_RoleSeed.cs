using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorldCitiesAPI.Migrations
{
    public partial class RoleSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0da03fd9-1fdf-4f65-8ff7-50d0604b93cb", "88ed764c-ac8e-4eba-9379-2bc49a8e928a", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "431a56ec-c38d-4511-8d4d-fdbda7d15953", "ce330ea7-a2cf-4455-996e-3a67c745c76f", "Registered", "REGISTERED" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0da03fd9-1fdf-4f65-8ff7-50d0604b93cb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "431a56ec-c38d-4511-8d4d-fdbda7d15953");
        }
    }
}
