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
        private IUnitOfWork _unitOfWork;
        
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }
        public IActionResult Index()
        {
            List<Company> Companies = _unitOfWork.Company.GetAll().ToList();
            return View(Companies);
        }
        public IActionResult UpSert(int ?id) {

            //Create new CompanyVM 
            Company company= new Company() { };

            if (id!=null && id!=0)
            {
                //here if that i pass id then it is update not create so i populate Company details

                company = _unitOfWork.Company.Get(c => c.ID == id);
            }

            return View(company);
        }
        [HttpPost]
        //[ActionName("Upsert")] by defualt 

        public IActionResult UpSert( Company company)
        {


            if (ModelState.IsValid)
            {

                if(company.ID == 0)
                { 
                _unitOfWork.Company.Add(company);
                    TempData["Success"] = "Company Created Successfully";
                }
                else
                {
                    _unitOfWork.Company.Update(company);
                   
                    TempData["Success"] = "Company Updated Successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction("Index");

            }

            
            return View(company);
        }

        //public IActionResult Delete(int id) {

        //    Company Company = _unitOfWork.Company.Get(p => p.Id == id);

        //    return View(Company);
        //}
        //[HttpPost]
        //public IActionResult Delete(Company DeletedPrd) {

        //    _unitOfWork.Company.Delete(DeletedPrd);
        //    _unitOfWork.Save();

        //    TempData["Success"] = "Company Deleted Successfully";
        //    return RedirectToAction("Index");
        //}

        #region API Calls
        [HttpGet]
        public IActionResult GetAll() {

            List<Company> Companies = _unitOfWork.Company.GetAll().ToList();
            return Json(new { Data = Companies });
        }   
        [HttpDelete]
        public IActionResult Delete(int id) {

            Company DeletedCmp = _unitOfWork.Company.Get(p=>p.ID==id);
            if (DeletedCmp == null) {
                return Json(new { success = false, Message = "Error happend while deleting " });

            }
            _unitOfWork.Company.Delete(DeletedCmp);
            _unitOfWork.Save();

            TempData["Success"] = "Company Deleted Successfully";
            return Json(new { success=true,Message="Company Deleted Successfully" });

        }
        #endregion
    }
}
