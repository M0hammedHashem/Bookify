using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ECommerce.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace ECommerce.DataAccess.Data
{
    public class AppDbContext:IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
         public DbSet<OrderHeader> OrderHeaders { get; set; }
         public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id=1,Name="Action",DisplayOrder=1},
                new Category { Id=2,Name="Scifi",DisplayOrder=2},
                new Category { Id=3,Name="History",DisplayOrder=3}

                );

            modelBuilder.Entity<Company>().HasData(
                new Company { ID=1, Name="MCKL",City="Cairo",State="Cairo",StreetAddress= "MCKL Street", PhoneNumber="01028377370",PostalCode="66567" },
                new Company { ID=2, Name="ITI",City="Assuit",State= "Assuit", StreetAddress= "ITI Street", PhoneNumber="01084399341",PostalCode="72738" },
                new Company { ID=3, Name="IBM",City="Cairo",State="Cairo",StreetAddress= "IBM Street", PhoneNumber="01135322320",PostalCode="54243" },
                new Company { ID=4, Name="3ergany",City="Giza",State= "Giza", StreetAddress= "3ergany Street", PhoneNumber="01054533222",PostalCode="98767" }


                );

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
        }

    }
    
   
}
