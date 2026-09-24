using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedMoreProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "Name", "Origin", "Price", "Stock" },
                values: new object[,]
                {
                    { 4, 1, null, null, "西達摩", "衣索比亞", 420m, 0 },
                    { 5, 1, null, null, "曼特寧", "印尼", 400m, 30 },
                    { 6, 3, null, null, "綜合配方豆", "綜合", 299m, 50 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}
