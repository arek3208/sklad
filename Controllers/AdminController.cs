using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using sklad.Data;
using sklad.Models;

namespace sklad.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();
            users.Remove(await _userManager.GetUserAsync(User));
            return View(users);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            var model = new ApplicationUserViewModel { ApplicationUser = user, IsAdmin = await _userManager.IsInRoleAsync(user,"Admin"), IsWarehouseman = await _userManager.IsInRoleAsync(user, "Warehouseman"), IsDriver = await _userManager.IsInRoleAsync(user, "Driver") };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(ApplicationUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.ApplicationUser.Id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.FirstName = model.ApplicationUser.FirstName;
                user.LastName = model.ApplicationUser.LastName;
                user.PhoneNumber = model.ApplicationUser.PhoneNumber;
                user.Company = model.ApplicationUser.Company;
                List<String> roles = new List<string>();
                if (model.IsAdmin) roles.Add("Admin");
                if (model.IsDriver) roles.Add("Driver");
                if (model.IsWarehouseman) roles.Add("Warehouseman");
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.AddToRolesAsync(user, roles.Except(currentRoles));
                await _userManager.RemoveFromRolesAsync(user, currentRoles.Except(roles));
                await _userManager.UpdateAsync(user);
                await _db.SaveChangesAsync();
                return RedirectToAction("UserList");
            }
            return View(user);
        }

        public async Task<IActionResult> DeleteUser(string id, bool? saveChangesError = false)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Błąd usuwania.";
            }
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                await _userManager.DeleteAsync(user);
                await _db.SaveChangesAsync();
            }
            catch (DataException)
            {
                return RedirectToAction("DeleteUser", new { id, saveChangesError = true });
            }
            return RedirectToAction("UserList");
        }
    }
}