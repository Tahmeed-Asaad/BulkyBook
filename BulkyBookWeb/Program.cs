using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBookWeb.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Dependency Injections added here
builder.Services.AddControllersWithViews();

//Linking the database connectionstring of appsettings.json. Also linking the dbcontext class to program.cs 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    
    builder.Configuration.GetConnectionString("DefaultConnection")));

//Injecting IRepository using Scoped. The other two is singleton and transient

//builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

//Instead of Category Repository,now using UnitofWork for efficinecy

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Helps to to add changes in layout.cshtml file from bootswatch Theme
builder.Services.AddRazorPages().AddRazorRuntimeCompilation(); 

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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Home}/{action=Index}/{id?}");
    //new pattern after including areas of admin and customer
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
