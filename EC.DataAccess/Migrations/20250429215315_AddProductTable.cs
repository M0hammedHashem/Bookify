using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ISBN = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ListPrice = table.Column<double>(type: "float", nullable: false),
                    Price = table.Column<double>(type: "float", nullable: false),
                    Price50 = table.Column<double>(type: "float", nullable: false),
                    Price100 = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title" },
                values: new object[,]
                {
                    { 1, "John Doe", "An epic journey through magical lands", "978-3-16-148410-0", 25.989999999999998, 23.989999999999998, 15.99, 20.989999999999998, "The Great Adventure" },
                    { 2, "Jane Smith", "Introduction to computer programming", "978-1-23-456789-0", 49.990000000000002, 44.990000000000002, 34.990000000000002, 39.990000000000002, "Programming Basics" },
                    { 3, "Alan Turing", "A thrilling tech detective story", "978-0-12-345678-9", 29.949999999999999, 26.5, 21.989999999999998, 24.0, "Mystery of the Code" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
