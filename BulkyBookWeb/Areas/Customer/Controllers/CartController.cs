using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
       private readonly IUnitOfWork _db;
        
       [BindProperty]
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
                ListCart = _db.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            }; 

            foreach(var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal + (cart.Count * cart.Price);
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
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _db.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties: "Product"),
                OrderHeader = new()
            };


            ShoppingCartVM.OrderHeader.ApplicationUser = _db.ApplicationUser.GetFirstorDefault(u => u.Id == claim.Value);

            ShoppingCartVM.OrderHeader.Name = ShoppingCartVM.OrderHeader.ApplicationUser.Name;
            ShoppingCartVM.OrderHeader.City = ShoppingCartVM.OrderHeader.ApplicationUser.City;
            ShoppingCartVM.OrderHeader.PhoneNumber = ShoppingCartVM.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVM.OrderHeader.State = ShoppingCartVM.OrderHeader.ApplicationUser.State;
            ShoppingCartVM.OrderHeader.StreetAddess = ShoppingCartVM.OrderHeader.ApplicationUser.StreetAdress;
            ShoppingCartVM.OrderHeader.PostalCode = ShoppingCartVM.OrderHeader.ApplicationUser.PostalCode;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal + (cart.Count * cart.Price);
            }

            
            return View(ShoppingCartVM);
        }


       

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPOST(ShoppingCartVM ShoppingCartVM)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM.ListCart = _db.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value, includeProperties:"Product");

            ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            ShoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserId = claim.Value;
          
            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBasedOnQuantity(cart.Count, cart.Product.Price, cart.Product.Price50, cart.Product.Price100);
                ShoppingCartVM.OrderHeader.OrderTotal = ShoppingCartVM.OrderHeader.OrderTotal + (cart.Count * cart.Price);
            }

            _db.OrderHeader.Add(ShoppingCartVM.OrderHeader);
            _db.Save();

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductId,
                    OrderId = ShoppingCartVM.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count

                };

                _db.OrderDetail.Add(orderDetail);
                _db.Save();
            }

            //Stripe Code.

            var domain = "https://localhost:7049/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes =new List<string>
                {
                    "card",
                },

                LineItems = new List<SessionLineItemOptions>(),
       
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/index",
            };

            foreach(var item in ShoppingCartVM.ListCart)
            {

                
                var sessionLineItem=new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price*100),//20.00-> 2000,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Title

                        },

                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(sessionLineItem);
        
            }

            var service = new SessionService();
            Session session = service.Create(options);

            //ShoppingCartVM.OrderHeader.SessionId = session.Id;
            //ShoppingCartVM.OrderHeader.PaymentIntentId = session.PaymentIntentId;

            _db.OrderHeader.UpdateStripePaymentId(ShoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _db.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


            //_db.ShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
           // _db.Save();
           // return RedirectToAction("Index", "Home");
        }

        public IActionResult OrderConfirmation(int id)

        {
            OrderHeader orderHeader = _db.OrderHeader.GetFirstorDefault(u => u.Id == id);

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if (session.PaymentStatus.ToLower() == "paid")
            {
                _db.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                _db.Save();
            }

            List<ShoppingCart> shoppingCarts = _db.ShoppingCart.GetAll(u=>u.ApplicationUserId==orderHeader.ApplicationUserId).ToList();

            _db.ShoppingCart.RemoveRange(shoppingCarts);
            _db.Save();
            return View(id);
        }

    }
  }


