using ECommerce.DataAccess.Repository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Ecommerce.Web.Areas.Customer.Controllers
{
    //[Authorize(Roles="Customer")]

    [Area("Customer")]
    public class CartController : Controller
    {
        public readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM shoppingCartVM { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
             shoppingCartVM = new ShoppingCartVM() {OrderHeader=new OrderHeader { } };
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var id= claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sh => sh.ApplicationUserID == id,include:"Product").ToList();

            foreach(var cart in  shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnPrdQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(shoppingCartVM);
        } 
        
        public IActionResult Summary()
        {

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var id = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

             shoppingCartVM = new ShoppingCartVM() { OrderHeader = new OrderHeader { },
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(sh => sh.ApplicationUserID == id, include: "Product").ToList()
            };
            var applicationUser = _unitOfWork.ApplicationUser.Get(u=>u.Id == id);
            shoppingCartVM.OrderHeader.ApplicationUser = applicationUser;
            shoppingCartVM.OrderHeader.Name = applicationUser.Name;
            shoppingCartVM.OrderHeader.StreetAddress = applicationUser.StreetAddrees;
            shoppingCartVM.OrderHeader.PostalCode = applicationUser.PostalCode;
            shoppingCartVM.OrderHeader.State = applicationUser.State;
            shoppingCartVM.OrderHeader.City = applicationUser.City;
            shoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnPrdQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }
            return View(shoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        public IActionResult SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            shoppingCartVM.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == userId,
                include: "Product").ToList();

            shoppingCartVM.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserID = userId;

            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);


            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnPrdQuantity(cart);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                //it is a regular customer 
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                //it is a company user
                shoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                shoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeader.Add(shoppingCartVM.OrderHeader);
            _unitOfWork.Save();
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                OrderDetail orderDetail = new()
                {
                    ProductId = cart.ProductID,
                    OrderHeaderId = shoppingCartVM.OrderHeader.ID,
                    Price = cart.Price,
                    Count = cart.Count
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {

                //it is a regular customer account and we need to capture payment
                //stripe logic

                var domain = Request.Scheme + "://" + Request.Host.Value + "/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.ID}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in shoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.50 => 2050
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                var service = new SessionService();
                Session session = service.Create(options);
                _unitOfWork.OrderHeader.UpdateStripePaymentID(shoppingCartVM.OrderHeader.ID , session.Id, session.PaymentIntentId);
                _unitOfWork.Save();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVM.OrderHeader.ID });
        }


        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderHeader = _unitOfWork.OrderHeader.Get(u => u.ID == id, include: "ApplicationUser");
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                //this is an order by customer

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    _unitOfWork.OrderHeader.UpdateStatus(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    _unitOfWork.Save();
                }
                HttpContext.Session.Clear();


            }

            List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart
                .GetAll(u => u.ApplicationUserID == orderHeader.ApplicationUserID).ToList();

            _unitOfWork.ShoppingCart.DeleteRange(shoppingCarts);
            _unitOfWork.Save();
            return View(id);
        }
        private double GetPriceBasedOnPrdQuantity(ShoppingCart shoppingCart)
        {
            double price = 0;
            if(shoppingCart.Count <=50)
            {
             price = shoppingCart.Product.Price;

            }
            else if(shoppingCart.Count >50 &&  shoppingCart.Count <=100)
            {

                price = shoppingCart.Product.Price50;

            }
            else
            {
                price = shoppingCart.Product.Price100;

            }

            return  price;
        }

      
        public IActionResult Minus(int id) { 
            ShoppingCart CartFromDb = _unitOfWork.ShoppingCart.Get(sh=>sh.ID == id);

            if (CartFromDb.Count <= 1)
            {
                var claimIdentity = (ClaimsIdentity)User.Identity;
                var applicationUserID = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                //remove
                HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u=>u.ApplicationUserID==applicationUserID).Count() - 1);
                
                _unitOfWork.ShoppingCart.Delete(CartFromDb);

            }
            else
            {
                CartFromDb.Count--;
                _unitOfWork.ShoppingCart.Update(CartFromDb);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Plus(int id) {
            ShoppingCart CartFromDb = _unitOfWork.ShoppingCart.Get(sh => sh.ID == id);
            CartFromDb.Count++;
            _unitOfWork.ShoppingCart.Update(CartFromDb);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }
        public IActionResult Remove(int id) {
            ShoppingCart CartFromDb = _unitOfWork.ShoppingCart.Get(sh => sh.ID == id);

            var claimIdentity = (ClaimsIdentity)User.Identity;
            var applicationUserID = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            //remove
            HttpContext.Session.SetInt32(SD.SessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == applicationUserID).Count() - 1);


            _unitOfWork.ShoppingCart.Delete(CartFromDb);

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));

        }


    }
}
