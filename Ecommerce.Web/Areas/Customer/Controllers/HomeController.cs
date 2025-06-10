using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bookify.Models;
using Bookify.DataAccess.Repository;
using Bookify.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Bookify.Models;
using System.Security.Claims;


namespace Bookify.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    //[Authorize(Roles =SD.Role_Customer)]

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var cartCount = await _unitOfWork.ShoppingCart
                    .GetAllAsync(u => u.ApplicationUserID == claim.Value);
                HttpContext.Session.SetInt32(SD.SessionCart, cartCount.Count());
            }

            var products = await _unitOfWork.Product.GetAllAsync(include:"Category,ProductImages");
            return View(products);
        }

        public async Task<IActionResult> Details(int ProductID)
        {
            var product = await _unitOfWork.Product
                .GetAsync(p => p.Id == ProductID, include: "Category,ProductImages");

            var shoppingCart = new ShoppingCart
            {
                Product = product,
                Count = 1,
                ProductID = product.Id,
               Price = product.Price
                
            };

            return View(shoppingCart);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            shoppingCart.ApplicationUserID = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var cartFromDb = await _unitOfWork.ShoppingCart
                .GetAsync(c => c.ProductID == shoppingCart.ProductID &&
                              c.ApplicationUserID == shoppingCart.ApplicationUserID);

            if (cartFromDb != null)
            {
                cartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                shoppingCart.Product= await _unitOfWork.Product
              .GetAsync(p => p.Id ==shoppingCart.ProductID, include: "Category,ProductImages");

                shoppingCart.Price = shoppingCart.Count * shoppingCart.Product.Price;
                await _unitOfWork.ShoppingCart.AddAsync(shoppingCart);
                await _unitOfWork.SaveAsync();
                var userCartCount = await _unitOfWork.ShoppingCart
                    .GetAllAsync(u => u.ApplicationUserID == shoppingCart.ApplicationUserID);
                HttpContext.Session.SetInt32(SD.SessionCart, userCartCount.Count());
            }

            await _unitOfWork.SaveAsync();
            TempData["success"] = "Cart updated successfully";

            return RedirectToAction("Index");
        }

     
    }
}
