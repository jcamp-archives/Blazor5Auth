using Microsoft.EntityFrameworkCore.Migrations;

namespace Blazor5Auth.Server.Data.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "4aec65bf-8788-425c-975f-b12663ccaaf1", "7a4843e3-0d14-49bc-9d1a-6051f4ef6364", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "2ab9b860-6f23-4f9d-a7f9-5db8c024f15a", "6f216a94-3e0b-475b-b42d-4ac3585589eb", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2ab9b860-6f23-4f9d-a7f9-5db8c024f15a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4aec65bf-8788-425c-975f-b12663ccaaf1");
        }
    }
}
