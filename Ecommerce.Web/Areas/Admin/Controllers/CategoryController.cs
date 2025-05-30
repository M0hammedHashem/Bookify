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
        public IActionResult Index()
        {
            List<Category> categories = _unitOfWork.Category.GetAll().ToList();
            return View(categories);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category CatFromRequest)
        {
            if (ModelState.IsValid)
            {

                _unitOfWork.Category.Add(CatFromRequest);
                _unitOfWork.Save();
                TempData["Success"] = "Category Created Successfully";

                return RedirectToAction("Index", "Category");

            }
            return View("Create", CatFromRequest);

        }



        public IActionResult Edit(int id)
        {
            Category EditedCat = _unitOfWork.Category.Get(c => c.Id == id);
            if (EditedCat == null)
            {
                return NotFound();
            }

            return View(EditedCat);
        }

        [HttpPost]
        public IActionResult Edit(Category EditedCat)
        {
            if (EditedCat == null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Update(EditedCat);

            _unitOfWork.Save();
            TempData["Success"] = "Category Updated Successfully";

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {

            Category DeletedCat = _unitOfWork.Category.Get(c => c.Id == id);
            if (DeletedCat == null)
            {
                return NotFound();
            }


            return View(DeletedCat);
        }
        [HttpPost]
        public ActionResult Delete(Category DeletedCat)
        {


            if (DeletedCat == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(DeletedCat);
            _unitOfWork.Save();
            TempData["Success"] = "Category Deleted Successfully";

            return RedirectToAction("Index");
        }

    }
}
