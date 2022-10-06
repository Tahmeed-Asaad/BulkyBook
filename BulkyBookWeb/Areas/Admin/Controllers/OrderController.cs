using BulkyBook.DataAccess.Repository;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _db;

        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM = new OrderVM()
            {
                OrderHeader = _db.OrderHeader.GetFirstorDefault(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                OrderDetail = _db.OrderDetail.GetAll(u => u.OrderId == orderId, includeProperties: "Product")

            };

            return View(OrderVM);
        }

        [ActionName("Details")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Details_PAY_NOW()
        {
            //This method is for company users who will pay after order has been shipped wihin 30 days.
            OrderVM.OrderHeader = _db.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id, includeProperties: "ApplicationUser");
            OrderVM.OrderDetail =_db.OrderDetail.GetAll(u => u.OrderId == OrderVM.OrderHeader.Id, includeProperties: "Product");

            //Stripe Code.
            //Stripe code only applicable if user is not a company user

            var domain = "https://localhost:7049/";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string>
                    {
                        "card",
                    },

                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                SuccessUrl = domain + $"admin/order/PaymentConfirmation?orderHeaderid={OrderVM.OrderHeader.Id}",
                CancelUrl = domain + $"admin/order/details?orderId={OrderVM.OrderHeader.Id}",

            };

            foreach (var item in OrderVM.OrderDetail)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100),//20.00-> 2000,
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

            _db.OrderHeader.UpdateStripePaymentId(OrderVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
            _db.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }


        public IActionResult PaymentConfirmation(int orderHeaderid)

        {
            OrderHeader orderHeader = _db.OrderHeader.GetFirstorDefault(u => u.Id == orderHeaderid);
            if (orderHeader.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                //if condition checks if the user is a company user. If hes is then this block will execute.

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);



                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _db.OrderHeader.UpdateStripePaymentId(orderHeader.Id, session.Id, session.PaymentIntentId);
                    _db.OrderHeader.UpdateStatus(orderHeaderid, orderHeader.OrderStatus, SD.PaymentStatusApproved);
                    _db.Save();
                }

            }
            
            return View(orderHeaderid);
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]

        public IActionResult UpdateOrderDetails()
        {

            var orderHeaderFromDb = _db.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
            orderHeaderFromDb.Name = OrderVM.OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddess = OrderVM.OrderHeader.StreetAddess;
            orderHeaderFromDb.City = OrderVM.OrderHeader.City;
            orderHeaderFromDb.State = OrderVM.OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderVM.OrderHeader.PostalCode;

            if (OrderVM.OrderHeader.Carrier != null)
            {
                orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            }

            if (OrderVM.OrderHeader.TrackingNumber != null)
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            }

            _db.OrderHeader.Update(orderHeaderFromDb);
            _db.Save();
            TempData["Success"] = "Order Details has been updated successfully";


            return RedirectToAction("Details", "Order", new { orderId = orderHeaderFromDb.Id });
        }




        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + ", " + SD.Role_Employee)]
        [AutoValidateAntiforgeryToken]

        public IActionResult ShipOrder()
        {

            var orderHeaderFromDb = _db.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);

            orderHeaderFromDb.Carrier = OrderVM.OrderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
            orderHeaderFromDb.ShippingDate = DateTime.Now;
            orderHeaderFromDb.OrderStatus = SD.StatusShipped;

            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateTime.Now.AddDays(30);
            }

            _db.OrderHeader.Update(orderHeaderFromDb);
            _db.Save();
            TempData["Success"] = "Order  has been updated successfully";


            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize(Roles = SD.Role_Admin + ", " + SD.Role_Employee)]

        public IActionResult CancelOrder()
        {

            var orderHeaderFromDb = _db.OrderHeader.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);

            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusApproved)
            {
                //refund the customer if the payment is alreadly approved
                var options = new RefundCreateOptions
                {
                    Reason = RefundReasons.RequestedByCustomer,
                    PaymentIntent = orderHeaderFromDb.PaymentIntentId,
                   
                    //Charge = orderHeaderFromDb.PaymentIntentId,

                };

                var servie = new RefundService();
                Refund refund = servie.Create(options);

                _db.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusRefunded);
            }

            else
            {
                //both order and payment status cancelled for who havent paid yet
                _db.OrderHeader.UpdateStatus(orderHeaderFromDb.Id, SD.StatusCancelled, SD.StatusCancelled);
            }
                
                _db.Save();
                TempData["Success"] = "Order  has been cancelled successfully";


                return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
            }
        
    




        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + ", " + SD.Role_Employee)]
        [AutoValidateAntiforgeryToken]

        public IActionResult StartProcessing()
        {

            _db.OrderHeader.UpdateStatus(OrderVM.OrderHeader.Id, SD.StatusInProcess);
            _db.Save();
            TempData["Success"] = "Order Status has been shipped successfully";


            return RedirectToAction("Details", "Order", new { orderId = OrderVM.OrderHeader.Id });
        }




        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                //only employee and admin users can access all orders
                orderHeaders = _db.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                orderHeaders = _db.OrderHeader.GetAll(u=>u.ApplicationUserId==claim.Value , includeProperties: "ApplicationUser");
            }

            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusPending);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.PaymentStatusApproved);
                    break;
                default:
                    break;
            }

            return Json(new { data = orderHeaders });
        }

        #endregion
    }
}
