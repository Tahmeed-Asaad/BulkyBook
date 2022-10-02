using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
       private readonly IUnitOfWork _db;
       public ShoppingCartVM ShoppingCartVM { get; set; }

       public int OrderTotal { get; set; }

       public CartController(IUnitOfWork db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                    ListCart = _db.ShoppingCart.GetAll(u=>u.ApplicationUserId==claim.Value,includeProperties:"Product")
            }; 

            foreach(var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.CartTotal = ShoppingCartVM.CartTotal + (cart.Count * cart.Price);
            }


            return View(ShoppingCartVM);
        }

        private double GetPriceBasedOnQuantity(double quantity, double price, double price50, double price100)
        {
            if (quantity <= 50)
                return price;
            else if (quantity > 50 && quantity <= 100)
                return price50;
            else
                return price100;
            
        }

        public IActionResult Plus(int cartId)
        {
            var cart = _db.ShoppingCart.GetFirstorDefault(u => u.Id == cartId);
            _db.ShoppingCart.IncrementCount(cart, 1);
            _db.Save();
            return RedirectToAction(nameof(Index));
            
        }

        public IActionResult Minus(int cartId)
        {
            
            var cart = _db.ShoppingCart.GetFirstorDefault(u => u.Id == cartId);
            if (cart.Count <= 1)
            {
                _db.ShoppingCart.Remove(cart);
            }

            else
            {
                _db.ShoppingCart.DecrementCount(cart, 1);
            }
            _db.Save();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Remove(int cartId)
        {
            var cart = _db.ShoppingCart.GetFirstorDefault(u => u.Id == cartId);
            _db.ShoppingCart.Remove(cart);
            _db.Save();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Summary()
        {
            /*var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _db.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product")
            };

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.CartTotal = ShoppingCartVM.CartTotal + (cart.Count * cart.Price);
            }

            */
            return View();
        }
    }
}
