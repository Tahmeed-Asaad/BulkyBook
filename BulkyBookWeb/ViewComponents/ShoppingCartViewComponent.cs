using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using BulkyBookWeb.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents;

public class ShoppingCartViewComponent : ViewComponent
{
    private readonly IUnitOfWork _db;

    public ShoppingCartViewComponent(IUnitOfWork db)
    {
        _db = db;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {

        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        if (claim != null)
        {
            //means the user is logged in

            if (HttpContext.Session.GetInt32(SD.SessionCart) != null)
            {
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
            else
            {
                HttpContext.Session.SetInt32(SD.SessionCart, _db.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).ToList().Count);
                return View(HttpContext.Session.GetInt32(SD.SessionCart));
            }
        }

        else
        {
            //if no user  logged in then clear session variable
            HttpContext.Session.Clear();
            return View(0);

        }


    }
}
