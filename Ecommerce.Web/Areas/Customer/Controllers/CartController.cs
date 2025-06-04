using Bookify.DataAccess.Repository;
using Bookify.Models;
using Bookify.Models.ViewModels;
using Bookify.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Security.Claims;

namespace Bookify.Web.Areas.Customer.Controllers
{
    //[Authorize(Roles="Customer")]

    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;


            ShoppingCartVM = new()
            {
                ShoppingCartList = await _unitOfWork.ShoppingCart.GetAllAsync(u => u.ApplicationUserID == userId,
                include: "Product"),
                OrderHeader = new()
            };

            IEnumerable<ProductImage> productImages = await _unitOfWork.ProductImage.GetAllAsync();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Product.ProductImages = productImages.Where(u => u.ProductId == cart.Product.Id).ToList();
                cart.Price = GetPriceBasedOnPrdQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVM);
        }
        [Authorize]
        public async Task<IActionResult> Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM = new ShoppingCartVM()
            {
                OrderHeader = new OrderHeader(),
                ShoppingCartList = await _unitOfWork.ShoppingCart
                    .GetAllAsync(sh => sh.ApplicationUserID == userId, include: "Product")
            };

            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);
            ShoppingCartVM.OrderHeader.ApplicationUser = applicationUser;
            ShoppingCartVM.OrderHeader.Name = applicationUser.Name;
            ShoppingCartVM.OrderHeader.StreetAddress = applicationUser.StreetAddrees;
            ShoppingCartVM.OrderHeader.PostalCode = applicationUser.PostalCode;
            ShoppingCartVM.OrderHeader.State = applicationUser.State;
            ShoppingCartVM.OrderHeader.City = applicationUser.City;
            ShoppingCartVM.OrderHeader.PhoneNumber = applicationUser.PhoneNumber;

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnPrdQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += cart.Price * cart.Count;
            }

            return View(ShoppingCartVM);
        }

        [HttpPost]
        [ActionName("Summary")]
        [Authorize]
        public async Task<IActionResult> SummaryPOST()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCartVM.ShoppingCartList = await _unitOfWork.ShoppingCart
                .GetAllAsync(u => u.ApplicationUserID == userId, include: "Product");

            ShoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.OrderHeader.ApplicationUserID = userId;

            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                cart.Price = GetPriceBasedOnPrdQuantity(cart);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                // Regular customer
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                // Company user
                ShoppingCartVM.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                ShoppingCartVM.OrderHeader.OrderStatus = SD.StatusApproved;
            }

            await _unitOfWork.OrderHeader.AddAsync(ShoppingCartVM.OrderHeader);
            await _unitOfWork.SaveAsync();

            foreach (var cart in ShoppingCartVM.ShoppingCartList)
            {
                var orderDetail = new OrderDetail()
                {
                    ProductId = cart.ProductID,
                    OrderHeaderId = ShoppingCartVM.OrderHeader.ID,
                    Price = cart.Price,
                    Count = cart.Count
                };
                await _unitOfWork.OrderDetail.AddAsync(orderDetail);
            }
            await _unitOfWork.SaveAsync();

            if (applicationUser.CompanyID.GetValueOrDefault() == 0)
            {
                // Stripe payment for regular customers
                var domain = $"{Request.Scheme}://{Request.Host.Value}/";
                var options = new SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={ShoppingCartVM.OrderHeader.ID}",
                    CancelUrl = domain + "customer/cart/index",
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                };

                foreach (var item in ShoppingCartVM.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
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
                await _unitOfWork.OrderHeader.UpdateStripePaymentID(ShoppingCartVM.OrderHeader.ID, session.Id, session.PaymentIntentId);
                await _unitOfWork.SaveAsync();
                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = ShoppingCartVM.OrderHeader.ID });
        }
        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var orderHeader = await _unitOfWork.OrderHeader
                .GetAsync(u => u.ID == id, include: "ApplicationUser");

            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);

                if (session.PaymentStatus.ToLower() == "paid")
                {
                    _unitOfWork.OrderHeader.UpdateStripePaymentID(id, session.Id, session.PaymentIntentId);
                    await _unitOfWork.OrderHeader.UpdateStatusAsync(id, SD.StatusApproved, SD.PaymentStatusApproved);
                    await _unitOfWork.SaveAsync();
                }
            }

            HttpContext.Session.Clear();

            var shoppingCarts = await _unitOfWork.ShoppingCart
                .GetAllAsync(u => u.ApplicationUserID == orderHeader.ApplicationUserID);

            await _unitOfWork.ShoppingCart.DeleteRangeAsync(shoppingCarts);
            await _unitOfWork.SaveAsync();

            return View(id);
        }
        [Authorize]

        public async Task<IActionResult> Minus(int id)
        {
            var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(sh => sh.ID == id);

            if (cartFromDb.Count <= 1)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                var cartCount = (await _unitOfWork.ShoppingCart
                    .GetAllAsync(u => u.ApplicationUserID == userId)).Count() - 1;
                HttpContext.Session.SetInt32(SD.SessionCart, cartCount);

                await _unitOfWork.ShoppingCart.DeleteAsync(cartFromDb);
            }
            else
            {
                cartFromDb.Count--;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize]

        public async Task<IActionResult> Plus(int id)
        {
            var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(sh => sh.ID == id);
            cartFromDb.Count++;
            _unitOfWork.ShoppingCart.Update(cartFromDb);
            await _unitOfWork.SaveAsync();
            return RedirectToAction(nameof(Index));
        }
        [Authorize]

        public async Task<IActionResult> Remove(int id)
        {
            var cartFromDb = await _unitOfWork.ShoppingCart.GetAsync(sh => sh.ID == id);
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartCount = (await _unitOfWork.ShoppingCart
                .GetAllAsync(u => u.ApplicationUserID == userId)).Count() - 1;
            HttpContext.Session.SetInt32(SD.SessionCart, cartCount);

            await _unitOfWork.ShoppingCart.DeleteAsync(cartFromDb);
            await _unitOfWork.SaveAsync();

            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBasedOnPrdQuantity(ShoppingCart shoppingCart)
        {
            if (shoppingCart.Count <= 50)
            {
                return shoppingCart.Product.Price;
            }
            else if (shoppingCart.Count <= 100)
            {
                return shoppingCart.Product.Price50;
            }
            return shoppingCart.Product.Price100;
        }
    }
}
