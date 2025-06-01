using ECommerce.DataAccess.Data;
using ECommerce.DataAccess.Repository;
using ECommerce.Models;
using ECommerce.Models.ViewModels;
using ECommerce.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]

    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Company> companies = await _unitOfWork.Company.GetAllAsync();
            return View(companies);
        }

        public async Task<IActionResult> UpSert(int? id)
        {
            Company company = new Company();

            if (id != null && id != 0)
            {
                company = await _unitOfWork.Company.GetAsync(c => c.ID == id);
                if (company == null)
                {
                    return NotFound();
                }
            }

            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> UpSert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.ID == 0)
                {
                    await _unitOfWork.Company.AddAsync(company);
                    TempData["Success"] = "Company Created Successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                    TempData["Success"] = "Company Updated Successfully";
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            IEnumerable<Company> companies = await _unitOfWork.Company.GetAllAsync();
            return Json(new { Data = companies });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            Company deletedCmp = await _unitOfWork.Company.GetAsync(p => p.ID == id);
            if (deletedCmp == null)
            {
                return Json(new { success = false, Message = "Error occurred while deleting" });
            }

            await _unitOfWork.Company.DeleteAsync(deletedCmp);
            await _unitOfWork.SaveAsync();

            TempData["Success"] = "Company Deleted Successfully";
            return Json(new { success = true, Message = "Company Deleted Successfully" });
        }
        #endregion
    }
}
