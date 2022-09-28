using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;



namespace BulkyBookWeb.DataAccess;


    //To inherit DbContext We need to use Microsoft's Entity FrameWork Core
    public class ApplicationDbContext:DbContext
    {
        

         public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

        //Create Tables here

        public DbSet<Category> Categories { get; set; }
        public DbSet<CoverType> CoverTypes { get; set; }
    }

