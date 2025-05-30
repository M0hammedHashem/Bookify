using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
             table: "Products",
             columns: new[] { "Id", "Author", "Description", "ISBN", "ListPrice", "Price", "Price100", "Price50", "Title","CategoryID" },
             values: new object[,]
             {
                    { 1, "John Doe", "An epic journey through magical lands", "978-3-16-148410-0", 25.989999999999998, 23.989999999999998, 15.99, 20.989999999999998, "The Great Adventure",1 },
                    { 2, "Jane Smith", "Introduction to computer programming", "978-1-23-456789-0", 49.990000000000002, 44.990000000000002, 34.990000000000002, 39.990000000000002, "Programming Basics",2 },
                    { 3, "Alan Turing", "A thrilling tech detective story", "978-0-12-345678-9", 29.949999999999999, 26.5, 21.989999999999998, 24.0, "Mystery of the Code",1 }
             });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
