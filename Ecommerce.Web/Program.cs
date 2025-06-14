using Bookify.DataAccess.Data;
using Bookify.DataAccess.Repository;
using Bookify.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Bookify.Models;
using Stripe;
using Bookify.DataAccess;
using Bookify.Models;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDataAccessLayerServices();
builder.Services.AddModelsServices();


builder.Services.AddIdentity<IdentityUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});


builder.Services.AddAuthentication().AddFacebook(options=>

{

    options.AppId = builder.Configuration.GetSection("Facebook:AppId").Value;
    options.AppSecret = builder.Configuration.GetSection("Facebook:AppSecret").Value;


    }
);builder.Services.AddAuthentication().AddGoogle(options=>
{ 
    options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientID").Value;
    options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
    options.CallbackPath = "/signin-google";

    options.SaveTokens = true;

}
);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});



builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();    
app.UseAuthorization();
app.UseSession();
InitializeDatabase();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();



app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();

void InitializeDatabase()
{
    using (var scope = app.Services.CreateScope())
    {
        var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}