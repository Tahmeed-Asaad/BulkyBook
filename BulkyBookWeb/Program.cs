using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBookWeb.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Identity.UI.Services;
using Stripe;
using BulkyBook.DataAccess.DbInitializer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Dependency Injections added here
builder.Services.AddControllersWithViews();

//Linking the database connectionstring of appsettings.json. Also linking the dbcontext class to program.cs 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(

builder.Configuration.GetConnectionString("DefaultConnection")));

//Mapping StripeSettings class from utility to stripe section in appsettings.json for payment in shopping cart summary
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));

//DefaultIdentity code auto generated as we used Identity Scalffold

/*builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();*/

//Instead of Deafult Identity, Use Identity as we need more  roles for our users.Also, Custom Identity needs explicitly generated tokens

builder.Services.AddIdentity<IdentityUser, IdentityRole>().AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();



//Injecting IRepository using Scoped. The other two is singleton and transient

//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

//Instead of Category Repository,now using UnitofWork for efficinecy

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Adding Idbinitializer for seeding database to publish in azure

//builder.Services.AddScoped<IDbInitializer, DbInitializer>();

//Needed to add it to create roles in registration.cs
builder.Services.AddSingleton<IEmailSender, EmailSender>();

//Helps to to add changes in layout.cshtml file from bootswatch Theme
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();

//Adding Facebook login Authorization

builder.Services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = "480569667446723";
    options.AppSecret = "fb8fb90ff583afe46f6b3e197f892778";
});

//Adding Components for Hot Reload

// Add services to the container.
//builder.Services.AddServerSideBlazor(); // Add support for Blazor
//builder.Services.AddRazorPages();       // Add support for .Net Core Razor Pages


//Following is needed to maintain default paths for Un-Authorized Users

builder.Services.ConfigureApplicationCookie(options =>

{
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath= $"/Identity/Account/Logout";
    options.AccessDeniedPath= $"/Identity/Account/AccessDenied";

}
) ;

//Adding Session

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession( options=>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(100);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    }

    );


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

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
//SeedDatabase();

//Authentication came from adding identity scaffold item. Middleware order is important. Authentication should come before
app.UseAuthentication();;

app.UseAuthorization();

//Session in pipeline
app.UseSession();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    //new pattern after including areas of admin and customer
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();


void SeedDatabase()
{   
    using(var scope= app.Services.CreateScope())
    {
        var dbInitializer= scope.ServiceProvider.GetRequiredService<IDbInitializer>();
        dbInitializer.Initialize();
    }
}