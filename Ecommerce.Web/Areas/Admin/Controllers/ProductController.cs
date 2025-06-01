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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UpSert(int? id)
        {
            ProductVM productVM = new()
            {
                CategoryList = (await _unitOfWork.Category.GetAllAsync())
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }).ToList(),
                Product = new Product()
            };

            if (id != null && id != 0)
            {
                productVM.Product = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            }

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpSert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(rootPath, @"images\product");

                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        string oldImgPath = Path.Combine(rootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImgPath))
                        {
                            System.IO.File.Delete(oldImgPath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, filename), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\product\" + filename;
                }

                if (productVM.Product.Id == 0)
                {
                    await _unitOfWork.Product.AddAsync(productVM.Product);
                    TempData["Success"] = "Product Created Successfully";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["Success"] = "Product Updated Successfully";
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }

            // If we got this far, something failed, redisplay form
            productVM.CategoryList = (await _unitOfWork.Category.GetAllAsync())
                .Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }).ToList();

            return View(productVM);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _unitOfWork.Product.GetAllAsync(include: "Category");
            return Json(new { data = products });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var productToDelete = await _unitOfWork.Product.GetAsync(p => p.Id == id);
            if (productToDelete == null)
            {
                return Json(new { success = false, Message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath,
                productToDelete.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            await _unitOfWork.Product.DeleteAsync(productToDelete);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Product Deleted Successfully";
            return Json(new { success = true, Message = "Product Deleted Successfully" });
        }
        #endregion
    }
}
