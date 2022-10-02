using BulkyBook.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace BulkyBookWeb.DataAccess;


//To inherit DbContext We need to use Microsoft's Entity FrameWork Core
//30-09-2021. Now I need to use Idenetity Class Library for login registration and user tracking. So I am gonna use IdentityDbContext
public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    //Create Tables here

    public DbSet<Category> Categories { get; set; }
    public DbSet<CoverType> CoverTypes { get; set; }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    public DbSet<Company> Companies { get;set; }

    public DbSet<Product> Products { get; set; }

    public DbSet<ShoppingCart> ShoppingCarts { get; set; }
 
}

