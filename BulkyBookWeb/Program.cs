using BulkyBookWeb.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//Dependency Injections added here
builder.Services.AddControllersWithViews();
//Linking the database connectionstring of appsettings.json. Also linking the dbcontext class to program.cs 
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    
    builder.Configuration.GetConnectionString("DefaultConnection")));

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
