using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Bookify.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace Bookify.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>().HasData(
    new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
    new Category { Id = 2, Name = "SciFi", DisplayOrder = 2 },
    new Category { Id = 3, Name = "History", DisplayOrder = 3 },
    new Category { Id = 4, Name = "Romance", DisplayOrder = 4 },
    new Category { Id = 5, Name = "Fantasy", DisplayOrder = 5 },
    new Category { Id = 6, Name = "Mystery", DisplayOrder = 6 }
);

            modelBuilder.Entity<Product>().HasData(
                // Action
                new Product
                {
                    Id = 1,
                    Title = "The Great Adventure",
                    Description = "An epic journey through magical lands.",
                    ISBN = "978-316-148410-0",
                    Author = "John Doe",
                    ListPrice = 25.99,
                    Price = 23.99,
                    Price50 = 20.99,
                    Price100 = 15.99,
                    CategoryID = 1
                    
                },
                // SciFi
                new Product
                {
                    Id = 2,
                    Title = "Dune",
                    Description = "A sci-fi masterpiece about desert politics and survival.",
                    ISBN = "978-044-117271-9",
                    Author = "Frank Herbert",
                    ListPrice = 18.99,
                    Price = 16.99,
                    Price50 = 14.99,
                    Price100 = 12.99,
                    CategoryID = 2
                },
                // History
                new Product
                {
                    Id = 3,
                    Title = "Sapiens: A Brief History of Humankind",
                    Description = "Explores the history of human evolution.",
                    ISBN = "978-006-231609-7",
                    Author = "Yuval Noah Harari",
                    ListPrice = 22.99,
                    Price = 19.99,
                    Price50 = 17.99,
                    Price100 = 15.99,
                    CategoryID = 3
                },
                // Romance
                new Product
                {
                    Id = 4,
                    Title = "Pride and Prejudice",
                    Description = "A classic romance about Elizabeth Bennet and Mr. Darcy.",
                    ISBN = "978-014-143951-8",
                    Author = "Jane Austen",
                    ListPrice = 12.99,
                    Price = 10.99,
                    Price50 = 8.99,
                    Price100 = 6.99,
                    CategoryID = 4
                },
                // Fantasy
                new Product
                {
                    Id = 5,
                    Title = "The Hobbit",
                    Description = "A fantasy adventure of Bilbo Baggins and the dragon Smaug.",
                    ISBN = "978-034-533968-3",
                    Author = "J.R.R. Tolkien",
                    ListPrice = 15.99,
                    Price = 13.99,
                    Price50 = 11.99,
                    Price100 = 9.99,
                    CategoryID = 5
                },
                // Mystery
                new Product
                {
                    Id = 6,
                    Title = "The Girl with the Dragon Tattoo",
                    Description = "A gripping mystery about a hacker and a journalist.",
                    ISBN = "978-030-726975-1",
                    Author = "Stieg Larsson",
                    ListPrice = 16.99,
                    Price = 14.99,
                    Price50 = 12.99,
                    Price100 = 10.99,
                    CategoryID = 6
                }
            );


            //modelBuilder.Entity<Category>().HasData(
            //    new Category { Id = 1, Name = "Action", DisplayOrder = 1 },
            //    new Category { Id = 2, Name = "Scifi", DisplayOrder = 2 },
            //    new Category { Id = 3, Name = "History", DisplayOrder = 3 }

            //    );

            modelBuilder.Entity<Company>().HasData(
                new Company { ID = 1, Name = "MCKL", City = "Cairo", State = "Cairo", StreetAddress = "MCKL Street", PhoneNumber = "01028377370", PostalCode = "66567" },
                new Company { ID = 2, Name = "ITI", City = "Assuit", State = "Assuit", StreetAddress = "ITI Street", PhoneNumber = "01084399341", PostalCode = "72738" },
                new Company { ID = 3, Name = "IBM", City = "Cairo", State = "Cairo", StreetAddress = "IBM Street", PhoneNumber = "01135322320", PostalCode = "54243" },
                new Company { ID = 4, Name = "3ergany", City = "Giza", State = "Giza", StreetAddress = "3ergany Street", PhoneNumber = "01054533222", PostalCode = "98767" }


                );
            /*
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Title = "The Great Adventure",
                    Description = "An epic journey through magical lands",
                    ISBN = "978-3-16-148410-0",
                    Author = "John Doe",
                    ListPrice = 25.99,
                    Price = 23.99,
                    Price50 = 20.99,
                    Price100 = 15.99,CategoryID = 1,
                    ImageUrl = ""
                },
                  new Product
                {
                      Id = 2,
                      Title = "Programming Basics",
                      Description = "Introduction to computer programming",
                      ISBN = "978-1-23-456789-0",
                      Author = "Jane Smith",
                      ListPrice = 49.99,
                      Price = 44.99,
                      Price50 = 39.99,
                      Price100 = 34.99,CategoryID= 2,
                      ImageUrl = ""
                  },
                  new Product
                {
                      Id = 3,
                      Title = "Mystery of the Code",
                      Description = "A thrilling tech detective story",
                      ISBN = "978-0-12-345678-9",
                      Author = "Alan Turing",
                      ListPrice = 29.95,
                      Price = 26.50,
                      Price50 = 24.00,
                      Price100 = 21.99, CategoryID=1    ,ImageUrl=""
                  }

                );

    
 */
        }
    }
}
