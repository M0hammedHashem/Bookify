using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ECommerce.Models;
using ECommerce.DataAccess.Repository;
using ECommerce.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Utility;
using System.Security.Claims;


namespace Ecommerce.Web.Areas.Customer.Controllers
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

        public IActionResult Index()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);
        

                if(claim!=null)
            {
                HttpContext.Session.SetInt32(SD.SessionCart,
               _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == claim.Value).Count());

            }
            var Products=_unitOfWork.Product.GetAll().ToList();
            return View(Products);

        }
       
        public IActionResult Details(int ProductID)
        {
            Product product= _unitOfWork.Product.Get(p=>p.Id==ProductID,include:"Category");
            
            ShoppingCart shoppingCart = new ShoppingCart { Product = product,Count=1,ProductID=product.Id };
            return View(shoppingCart);
        }
        [Authorize]

        [HttpPost]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            shoppingCart.ApplicationUserID = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ShoppingCart CartFromDb = _unitOfWork.ShoppingCart.Get(c=>c.ProductID==shoppingCart.ProductID && c.ApplicationUserID==shoppingCart.ApplicationUserID);
            if (CartFromDb != null)
            {
                CartFromDb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(CartFromDb);
                _unitOfWork.Save();

            }
            else
            {
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(SD.SessionCart,
               _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserID == shoppingCart.ApplicationUserID).Count());

            }
            TempData["success"] = "Cart updated successfully";


            return RedirectToAction("Index");
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
}
