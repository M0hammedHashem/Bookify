using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCompanyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "ID", "City", "Name", "PhoneNumber", "PostalCode", "State", "StreetAddress" },
                values: new object[,]
                {
                    { 1, "Cairo", "MCKL", "01028377370", "66567", "Cairo", "MCKL Street" },
                    { 2, "Assuit", "ITI", "01084399341", "72738", "Assuit", "ITI Street" },
                    { 3, "Cairo", "IBM", "01135322320", "54243", "Cairo", "IBM Street" },
                    { 4, "Giza", "3ergany", "01054533222", "98767", "Giza", "3ergany Street" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "ID",
                keyValue: 4);
        }
    }
}
