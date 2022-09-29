using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BulkyBookWeb.Areas.Customer.Controllers;
[Area("Customer")]

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IUnitOfWork _db;

    public HomeController(ILogger<HomeController> logger, IUnitOfWork db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        IEnumerable<Product> productList = _db.Product.GetAll(includeProperties: "Category,CoverType");
        return View(productList);
    }

    public IActionResult Details(int id)
    {
        ShoppingCart cartObj = new()
        {
            Count = 1,
            Product = _db.Product.GetFirstorDefault(u => u.Id == id, includeProperties: "Category,CoverType")
        };
    return View(cartObj);
}
    
    

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
