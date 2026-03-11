using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Coordinator.Migrations
{
    /// <inheritdoc />
    public partial class created_seedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Nodes",
                columns: new[] { "Id", "microserviceName" },
                values: new object[,]
                {
                    { new Guid("556b343d-a2ab-49e0-a8da-7a8ce9df03c1"), "Payment.API" },
                    { new Guid("bdf95ee8-3c10-4541-b1e2-a7f49213896b"), "Stock.API" },
                    { new Guid("dc807c8e-d1fd-426e-a68e-201a7dbf4188"), "Order.API" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("556b343d-a2ab-49e0-a8da-7a8ce9df03c1"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("bdf95ee8-3c10-4541-b1e2-a7f49213896b"));

            migrationBuilder.DeleteData(
                table: "Nodes",
                keyColumn: "Id",
                keyValue: new Guid("dc807c8e-d1fd-426e-a68e-201a7dbf4188"));
        }
    }
}
