using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EventTypes",
                columns: new[] { "Id", "EventTypeName" },
                values: new object[,]
                {
                    { 1, "Екзамен" },
                    { 2, "Контрольна робота" },
                    { 3, "Шкільні заходи" },
                    { 4, "Батьківські збори" },
                    { 5, "Особисті події" }
                });

            migrationBuilder.UpdateData(
                table: "ParentTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "ParentTypeName",
                value: "Батько");

            migrationBuilder.UpdateData(
                table: "ParentTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "ParentTypeName",
                value: "Мати");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EventTypes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "ParentTypes",
                keyColumn: "Id",
                keyValue: 1,
                column: "ParentTypeName",
                value: "Father");

            migrationBuilder.UpdateData(
                table: "ParentTypes",
                keyColumn: "Id",
                keyValue: 2,
                column: "ParentTypeName",
                value: "Mother");
        }
    }
}
