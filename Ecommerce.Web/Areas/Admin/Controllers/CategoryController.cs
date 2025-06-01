using Microsoft.AspNetCore.Mvc;
using ECommerce.DataAccess.Data;
using ECommerce.Models;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Utility;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]

    [Area("Admin")]


    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unit)
        {
            _unitOfWork = unit;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAllAsync();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category CatFromRequest)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.Category.AddAsync(CatFromRequest);
                await _unitOfWork.SaveAsync();
                TempData["Success"] = "Category Created Successfully";
                return RedirectToAction("Index");
            }
            return View(CatFromRequest);
        }

        public async Task<IActionResult> Edit(int id)
        {
            Category EditedCat = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (EditedCat == null)
            {
                return NotFound();
            }
            return View(EditedCat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category EditedCat)
        {
            if (EditedCat == null)
            {
                return NotFound();
            }

             _unitOfWork.Category.Update(EditedCat);
            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Category Updated Successfully";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            Category DeletedCat = await _unitOfWork.Category.GetAsync(c => c.Id == id);
            if (DeletedCat == null)
            {
                return NotFound();
            }
            return View(DeletedCat);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Category DeletedCat)
        {
            if (DeletedCat == null)
            {
                return NotFound();
            }
            await _unitOfWork.Category.DeleteAsync(DeletedCat);
            await _unitOfWork.SaveAsync();
            TempData["Success"] = "Category Deleted Successfully";
            return RedirectToAction("Index");
        }
    }

}
