using Bookify.DataAccess;
using Bookify.DataAccess.Repository.IRepository;
using Bookify.DataAccess.Repository;
using Bookify.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Bookify.DataAccess
{
    public static class ExtenssionDA
    {

        public static void AddDataAccessLayerServices(this IServiceCollection Services)
        {
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped( typeof(IRepository<>), typeof(Repository<>));

            Services.AddScoped<ICategoryRepository, CategoryRepository>();
            Services.AddScoped<IProductRepository, ProductRepository>();
            Services.AddScoped<ICompanyRepository, CompanyRepository>();
            Services.AddScoped<IApplicationUserRepository, ApplicationUserRepository>();
            Services.AddScoped<IShoppingCartRepository, ShoppingCartRepository>();
            Services.AddScoped<IOrderHeaderRepository, OrderHeaderRepository>();
            Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            Services.AddScoped<IDbInitializer, DbInitializer>();
            Services.AddScoped<IEmailSender, EmailSender>();
            Services.AddScoped<IProductImageRepository, ProductImageRepository>();

        }
    }
}
