using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using BulkyBookWeb.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.DbInitializer
{
   public class DbInitializer : IDbInitializer
    {


        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext db)
        {

            _roleManager = roleManager;
            _userManager = userManager;
            _db = db;
        }

        public void Initialize()
        {

            //create migrations if they are not applied

            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }

            catch(Exception ex)
            {

            }

            //create roles if they are not created


            //Manual Code. Without if condition everytime the role will be created.If condition ensures roles to be only if any 1 of the 4 roles are
            //not created.
            if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Indi)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_User_Comp)).GetAwaiter().GetResult();


                //if roles are not created, we will create admin user as well

               var result = _userManager.CreateAsync(new ApplicationUser
               {
                   UserName = "Admin123@t.com",
                   Email = "Admin123@t.com",
                   Name = "Md. Mizanur Rahman",
                   PhoneNumber = "+8801670835249",
                   StreetAdress = "31, Subol Das Road, Choudury Bazar, 4th Floor, Lalbag",
                   State = "Dhaka, Bangladesh",
                   PostalCode = "1211",
                   City = "Dhaka, Lalbag"
               }, "Admin123#").GetAwaiter().GetResult();

                if (result.Succeeded)
                {
                    Console.WriteLine("HURRAY");
                }

                ApplicationUser user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin123@t.com");


                _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();

            }

            return;


           
        }
    }
}
