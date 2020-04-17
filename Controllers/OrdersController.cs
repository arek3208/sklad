using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using sklad.Data;
using sklad.Extensions;
using sklad.Models;

namespace sklad.Controllers
{
	public class OrdersController : Controller
	{
		private readonly ApplicationDbContext _context;

		private readonly UserManager<ApplicationUser> _userManager;

		public OrdersController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
		{
			_context = context;
			_userManager = userManager;
		}

		[Authorize]
		public async Task<IActionResult> SelectAddress()
		{
			var user = await _userManager.GetUserAsync(User);
			var addresses = _context.Address.Include(a => a.ApplicationUser).Where(a => a.ApplicationUser == user);
			return View(addresses);
		}

	    [Authorize]
		public async Task<IActionResult> MakeOrder(int AddressId)
		{
			var Address = _context.Address.Find(AddressId);
			if (Address == null)
			{
				return NotFound();
			}
			Dictionary<int, int> cart = HttpContext.Session.GetObject<Dictionary<int, int>>("cart");
			if (cart == null)
			{
				return RedirectToAction("BrowseItems", "Items");
			}
			var user = await _userManager.GetUserAsync(User);
			var order = new Order { ClientId = user.Id, OrderItems = new List<OrderItem>(), AddressId = AddressId};
			foreach(var i in cart)
			{
				var item = await _context.Item.FindAsync(i.Key);
				var orderitem = new OrderItem { Amount=i.Value, Item=item, Order=order, Price = i.Value * item.Price};
				order.OrderItems.Add(orderitem);
				await _context.OrderItem.AddAsync(orderitem);
			}
			await _context.Order.AddAsync(order);
			await _context.SaveChangesAsync();
			HttpContext.Session.Remove("cart");
			return View(order);
		}

		// GET: Orders
		public async Task<IActionResult> Index()
		{
			var applicationDbContext = _context.Order.Include(o => o.Client).Include(o => o.Driver);
			return View(await applicationDbContext.ToListAsync());
		}

		// GET: Orders/Details/5
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var order = await _context.Order
				.Include(o => o.Client)
				.Include(o => o.Driver)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		// GET: Orders/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var order = await _context.Order.FindAsync(id);
			if (order == null)
			{
				return NotFound();
			}
			ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", order.ClientId);
			ViewData["DriverId"] = new SelectList(_context.Users, "Id", "Id", order.DriverId);
			return View(order);
		}

		// POST: Orders/Edit/5
		// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("Id,OrderDate,RealizationDate,ClientId,DriverId")] Order order)
		{
			if (id != order.Id)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(order);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!OrderExists(order.Id))
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
			ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", order.ClientId);
			ViewData["DriverId"] = new SelectList(_context.Users, "Id", "Id", order.DriverId);
			return View(order);
		}

		// GET: Orders/Delete/5
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var order = await _context.Order
				.Include(o => o.Client)
				.Include(o => o.Driver)
				.FirstOrDefaultAsync(m => m.Id == id);
			if (order == null)
			{
				return NotFound();
			}

			return View(order);
		}

		// POST: Orders/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var order = await _context.Order.FindAsync(id);
			_context.Order.Remove(order);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool OrderExists(int id)
		{
			return _context.Order.Any(e => e.Id == id);
		}
	}
}
