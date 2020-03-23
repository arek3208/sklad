using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using sklad.Data;
using sklad.Models;

namespace sklad.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CategoriesController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: Categories
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _db.Category.ToListAsync());
        }

        // GET: Categories/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            List<SelectListItem> CategoryListItems = new List<SelectListItem> { new SelectListItem {Text="Brak", Value="-1" } };
            foreach(var c in _db.Category)
            {
                CategoryListItems.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }
            ViewBag.Categories = CategoryListItems;
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Category model)
        {
            if (ModelState.IsValid)
            {
                Category category = new Category { Id=model.Id, Name=model.Name };
                category.CategoryId = model.CategoryId != -1 ? model.CategoryId : null;
                _db.Add(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Categories/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            List<SelectListItem> CategoryListItems = new List<SelectListItem> { new SelectListItem { Text = "Brak", Value = "-1" } };
            foreach (var c in _db.Category)
            {
                CategoryListItems.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString() });
            }
            CategoryListItems.RemoveAll(x => x.Value == category.Id.ToString());
            ViewBag.Categories = CategoryListItems;
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    Category category = new Category { Id = model.Id, Name = model.Name };
                    category.CategoryId = model.CategoryId != -1 ? model.CategoryId : null;
                    _db.Update(category);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Categories/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Category
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _db.Category.FindAsync(id);
            _db.DeleteCategory(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public JsonResult GetCategories()
        {
            var categories = _db.CategoriesToViewModel(_db.Category.Where(x => x.ParentCategory == null).Include(x => x.ChildCategories).ToList());
            return new JsonResult(categories);
        }

        private bool CategoryExists(int id)
        {
            return _db.Category.Any(e => e.Id == id);
        }

    }
}
