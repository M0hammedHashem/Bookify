using Microsoft.AspNetCore.Mvc;
using ECommerce.DataAccess.Data;
using ECommerce.Models;
using ECommerce.DataAccess.Repository;
using ECommerce.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using ECommerce.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using ECommerce.Models.ViewModels;

namespace Ecommerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(
            UserManager<IdentityUser> userManager,
            IUnitOfWork unitOfWork,
            RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> RoleManagment(string userId)
        {
            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId, include: "Company");
            var userRoles = await _userManager.GetRolesAsync(applicationUser);

            var roleVM = new RoleManagmentVM()
            {
                ApplicationUser = applicationUser,
                RoleList = _roleManager.Roles.Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Name
                }),
                CompanyList = (await _unitOfWork.Company.GetAllAsync()).Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.ID.ToString()
                })
            };

            return View(roleVM);
        }

        [HttpPost]
        public async Task<IActionResult> RoleManagment(RoleManagmentVM roleManagmentVM)
        {
            var applicationUser = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == roleManagmentVM.ApplicationUser.Id);
            var oldRoles = await _userManager.GetRolesAsync(applicationUser);
            var oldRole = oldRoles.FirstOrDefault();

            applicationUser.Name = roleManagmentVM.ApplicationUser.Name;

            if (roleManagmentVM.ApplicationUser.Role != oldRole)
            {
                // Role changed
                if (roleManagmentVM.ApplicationUser.Role == SD.Role_Company)
                {
                    applicationUser.CompanyID = roleManagmentVM.ApplicationUser.CompanyID;
                }
                if (oldRole == SD.Role_Company)
                {
                    applicationUser.CompanyID = null;
                }

                _unitOfWork.ApplicationUser.Update(applicationUser);
                await _unitOfWork.SaveAsync();

                await _userManager.RemoveFromRoleAsync(applicationUser, oldRole);
                await _userManager.AddToRoleAsync(applicationUser, roleManagmentVM.ApplicationUser.Role);
            }
            else
            {
                // Same role but company might have changed
                if (oldRole == SD.Role_Company &&
                    applicationUser.CompanyID != roleManagmentVM.ApplicationUser.CompanyID)
                {
                    applicationUser.CompanyID = roleManagmentVM.ApplicationUser.CompanyID;
                }
                _unitOfWork.ApplicationUser.Update(applicationUser);
                await _unitOfWork.SaveAsync();
            }

            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userList = await _unitOfWork.ApplicationUser.GetAllAsync(include: "Company");

            var usersWithRoles = new List<ApplicationUser>();
            foreach (var user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = roles.FirstOrDefault();

                if (user.Company == null)
                {
                    user.Company = new Company() { Name = "" };
                }

                usersWithRoles.Add(user);
            }

            return Json(new { data = usersWithRoles });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string id)
        {
            var user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == id);
            if (user == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (user.LockoutEnd != null && user.LockoutEnd > DateTime.Now)
            {
                // Unlock user
                user.LockoutEnd = DateTime.Now;
            }
            else
            {
                // Lock user
                user.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _unitOfWork.ApplicationUser.Update(user);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Operation Successful" });
        }

        #endregion
    }
}
