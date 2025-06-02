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
                productVM.Product = await _unitOfWork.Product.GetAsync(p => p.Id == id,include:"ProductImages");
            }

            return View(productVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpSert(ProductVM productVM, List<IFormFile>? files)
        {

         
                if (ModelState.IsValid)
                {
                    if (productVM.Product.Id == 0)
                    {
                        await _unitOfWork.Product.AddAsync(productVM.Product);

                        TempData["success"] = "Product Created Successfully";
                    }
                    else
                    {
                        _unitOfWork.Product.Update(productVM.Product);

                        TempData["success"] = "Product Updated Successfully";
                    }
                    await _unitOfWork.SaveAsync();
                    string wwwRootPath = _webHostEnvironment.WebRootPath;

                    if (files != null)
                    {

                        foreach (IFormFile file in files)
                        {
                            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            string productPath = @"images\products\product-" + productVM.Product.Id;
                            string finalPath = Path.Combine(wwwRootPath, productPath);

                            if (!Directory.Exists(finalPath))
                                Directory.CreateDirectory(finalPath);

                            using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                            {
                                file.CopyTo(fileStream);
                            }

                            ProductImage productImage = new()
                            {
                                ImageUrl = @"\" + productPath + @"\" + fileName,
                                ProductId = productVM.Product.Id,
                            };

                            if (productVM.Product.ProductImages == null)
                                productVM.Product.ProductImages = new List<ProductImage>();

                            productVM.Product.ProductImages.Add(productImage);

                        }

                        _unitOfWork.Product.Update(productVM.Product);
                        await _unitOfWork.SaveAsync();

                    }

                    TempData["success"] = "Product created/updated successfully";

                    return RedirectToAction("Index");
                }
                else
                {
                    productVM.CategoryList = (await _unitOfWork.Category.GetAllAsync()).Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });
                    return View(productVM);
                }


            
        }

        public async Task<IActionResult> DeleteImage(int imageId)
        {
            var imageToBeDeleted =await _unitOfWork.ProductImage.GetAsync(u => u.Id == imageId);
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null)
            {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
                {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

               await _unitOfWork.ProductImage.DeleteAsync(imageToBeDeleted);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "Deleted successfully";
            }

            return RedirectToAction(nameof(UpSert), new { id = productId });
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
            var productToBeDeleted = await _unitOfWork.Product.GetAsync(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, Message = "Error While Deleting" });
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

           await _unitOfWork.Product.DeleteAsync(productToBeDeleted);
           await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Delete Successful" });

        }
        #endregion
    }
}
