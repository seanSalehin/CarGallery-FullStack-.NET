using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class seed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Cars",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "CreatedDate", "Deatils", "ImageUrl", "Name", "Rate", "UpdatedDate", "price" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Compact sedan", "https://live.staticflickr.com/65535/49440475318_2d9d3c994d_o.jpg", "Toyota Corolla", 4.5, null, 20000 },
                    { 2, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sporty compact", "https://live.staticflickr.com/65535/52051754815_b5de4bea83_b.jpg", "Honda Civic", 4.7000000000000002, null, 22000 },
                    { 3, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Muscle car", "https://live.staticflickr.com/65535/52045457604_fc0a27e506_b.jpg", "Ford Mustang", 4.9000000000000004, null, 35000 },
                    { 4, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Performance coupe", "https://live.staticflickr.com/65535/52821278553_d03072cf41_b.jpg", "Chevrolet Camaro", 4.7999999999999998, null, 34000 },
                    { 5, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Luxury sedan", "https://live.staticflickr.com/65535/51016184159_d0fc8847be_b.jpg", "BMW 3 Series", 4.5999999999999996, null, 42000 },
                    { 6, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Comfortable sedan", "https://live.staticflickr.com/65535/50273443922_298b8bb6f8_b.jpg", "Audi A4", 4.5, null, 41000 },
                    { 7, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Premium sedan", "https://live.staticflickr.com/65535/51818320308_1769ae013c_b.jpg", "Mercedes C-Class", 4.5999999999999996, null, 43000 },
                    { 8, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Electric sedan", "https://live.staticflickr.com/65535/50195997318_00d4640d11_b.jpg", "Tesla Model 3", 4.9000000000000004, null, 50000 },
                    { 9, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Compact hatchback", "https://live.staticflickr.com/65535/50376941986_69e4f2056f_b.jpg", "Volkswagen Golf", 4.2999999999999998, null, 23000 },
                    { 10, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Midsize sedan", "https://live.staticflickr.com/65535/50733651236_abc123abcd_b.jpg", "Nissan Altima", 4.2000000000000002, null, 25000 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cars",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedDate",
                table: "Cars",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
