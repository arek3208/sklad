using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sklad.Data;
using sklad.Models;

namespace sklad.Controllers
{
	public class ItemsController : Controller
	{
		private readonly ApplicationDbContext _context;

		public ItemsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// GET: Items
		public async Task<IActionResult> Index()
		{
			var items = _context.Item.Include(i => i.Category);
			return View(await items.ToListAsync());
		}

		// GET: Items/Details/5
		public async Task<IActionResult> Details(int? id, bool order = false, int? category = null)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.Item
				.Include(i => i.Category)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (item == null)
			{
				return NotFound();
			}
			ViewBag.order = order;
			ViewBag.category = category;
			return View(item);
		}

		// GET: Items/Create
		[Authorize(Roles = "Admin,Warehouseman")]
		public IActionResult Create()
		{
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name");
			return View();
		}

		// POST: Items/Create
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Admin,Warehouseman")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Item item, IFormFile image)
		{
			if (ModelState.IsValid)
			{
				if (image != null && image.Length > 0)
				{
					UploadFile(ref item, image);
				}
				_context.Add(item);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", item.CategoryId);
			return View(item);
		}

		// GET: Items/Edit/5
		[Authorize(Roles = "Admin,Warehouseman")]
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.Item.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", item.CategoryId);
			return View(item);
		}

		// POST: Items/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[Authorize(Roles = "Admin,Warehouseman")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Item item, IFormFile image)
		{
			if (id != item.Id)
			{
				return NotFound();
			}

			var currentItem = _context.Item.AsNoTracking().First(x => x.Id == id);

			if (ModelState.IsValid)
			{
				try
				{
					if (image != null && image.Length > 0)
					{
						UploadFile(ref item, image);
					}
					else
					{
						item.Image = currentItem.Image;
					}
					_context.Update(item);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!ItemExists(item.Id))
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
			ViewData["CategoryId"] = new SelectList(_context.Category, "Id", "Name", item.CategoryId);
			return View(item);
		}

		// GET: Items/Delete/5
		[Authorize(Roles = "Admin,Warehouseman")]
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var item = await _context.Item
				.Include(i => i.Category)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (item == null)
			{
				return NotFound();
			}

			return View(item);
		}

		// POST: Items/Delete/5
		[Authorize(Roles = "Admin,Warehouseman")]
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var item = await _context.Item.FindAsync(id);
			if (item.Image != null)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\items", item.Image);
				if (System.IO.File.Exists(filePath))
				{
					System.IO.File.Delete(filePath);
				}
			}
			_context.Item.Remove(item);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult BrowseItems(int? id)
		{
			var categories = _context.Category;
			ViewBag.categories = categories;
			ViewBag.active = id;
			var items = _context.Item.Include(i => i.Category).AsQueryable();
			if (id != null)
			{
				items = items.Where(i => i.CategoryId == id);
			}
			return View(items);
		}

		private bool ItemExists(int id)
		{
			return _context.Item.Any(e => e.Id == id);
		}

		private void UploadFile(ref Item item, IFormFile image)
		{
				var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\items", fileName);
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					image.CopyTo(fileStream);
				}
				item.Image = fileName;
		}
	}
}
