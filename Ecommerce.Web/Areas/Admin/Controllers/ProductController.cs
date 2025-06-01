using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment= webHostEnvironment;
        }
        public IActionResult Index()
        {
            
            return View();
        }
        public IActionResult UpSert(int ?id) { 
            
            //Create new ProductVM 
            ProductVM productVM = new ProductVM();
           
            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
            productVM.Product = new Product();
            if (id!=null && id!=0)
            {
                //here if that i pass id then it is update not create so i populate product details
                productVM.Product=_unitOfWork.Product.Get(p=>p.Id==id);
               
            }

            return View(productVM);
        }
        [HttpPost]
        //[ActionName("Upsert")] by defualt 

        public IActionResult UpSert(ProductVM productVM, IFormFile ?file)
        {


            if (ModelState.IsValid)
            {

                string rootfile = _webHostEnvironment.WebRootPath;

                if (file != null) {

                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(rootfile, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        string oldImgPath = Path.Combine(rootfile, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + filename;

                }

                if(productVM.Product.Id == 0)
                { 
                _unitOfWork.Product.Add(productVM.Product);
                    TempData["Success"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                   
                    TempData["Success"] = "Product Updated Successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");

            }

            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();
            return View(productVM);
        }

        //public IActionResult Delete(int id) {

        //    Product product = _unitOfWork.Product.Get(p => p.Id == id);

        //    return View(product);
        //}
        //[HttpPost]
        //public IActionResult Delete(Product DeletedPrd) {

        //    _unitOfWork.Product.Delete(DeletedPrd);
        //    _unitOfWork.Save();

        //    TempData["Success"] = "Product Deleted Successfully";
        //    return RedirectToAction("Index");
        //}

        #region API Calls
        [HttpGet]
        public IActionResult GetAll()
        {
            
            
            List<Product> Products = _unitOfWork.Product.GetAll(
          include: "Category").ToList();
            return Json(new { data = Products });


            //return Json(new { Data = productsDTOs });
        }
        [HttpDelete]
        public IActionResult Delete(int id) {

            Product DeletedPrd = _unitOfWork.Product.Get(p=>p.Id==id);
            if (DeletedPrd == null) {
                return Json(new { success = false, Message = "Error happend while deleting " });

            }
            var oldImagePath=Path.Combine(_webHostEnvironment.WebRootPath,DeletedPrd.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath)) {

                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Delete(DeletedPrd);
            _unitOfWork.Save();

            TempData["Success"] = "Product Deleted Successfully";
            return Json(new { success=true,Message="Product Deleted Successfully" });

        }
        #endregion
    }
}
