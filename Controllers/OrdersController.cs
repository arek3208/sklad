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
			await _context.Order.AddAsync(order);
			foreach (var i in cart)
			{
				var item = await _context.Item.FindAsync(i.Key);
				if (i.Value < 1 || i.Value > item.Quantity)
				{
					return new ConflictResult();
				}
				item.Quantity -= i.Value;
				var orderitem = new OrderItem { Amount=i.Value, Item=item, Order=order, Price = i.Value * item.Price};
				await _context.OrderItem.AddAsync(orderitem);
				order.OrderItems.Add(orderitem);
			}
			await _context.SaveChangesAsync();
			HttpContext.Session.Remove("cart");
			return View(order);
		}

		// GET: Orders
		public async Task<IActionResult> MyOrders()
		{
			var user = await _userManager.GetUserAsync(User);
			var orders = _context.Order.Include(o => o.Client).Include(o => o.Driver).Include(o => o.Address).Where(o => o.ClientId == user.Id);
			return View(await orders.ToListAsync());
		}

		[Authorize(Roles = "Admin, Driver")]
		public async Task<IActionResult> AvailableOrders()
		{
			var user = await _userManager.GetUserAsync(User);
			var availableOrders = _context.Order.Include(o => o.Client).Include(o => o.OrderItems).ThenInclude(o => o.Item).Include(o => o.Address).Where(o => o.Driver == null).ToList();
			var myOrders = _context.Order.Include(o => o.Client).Include(o => o.OrderItems).ThenInclude(o => o.Item).Include(o => o.Address).Where(o => o.DriverId == user.Id).ToList();
			return View(new Tuple<List<Order>, List<Order>>(myOrders, availableOrders));
		}

		[Authorize(Roles = "Admin, Driver")]
		public async Task<IActionResult> DriverFinishedOrders()
		{
			var user = await _userManager.GetUserAsync(User);
			var orders = _context.Order.Include(o => o.Client).Include(o => o.Driver).Include(o => o.Address).Where(o => o.Driver.Id == user.Id && o.RealizationDate.HasValue);
			return View(orders);
		}

		[Authorize(Roles = "Admin, Driver")]
		public async Task<IActionResult> TakeOrder(int id)
		{
			var order = _context.Order.Find(id);
			if(order.Driver != null)
			{
				return BadRequest();
			}
			order.Driver = await _userManager.GetUserAsync(User);
			_context.Update(order);
			await _context.SaveChangesAsync();
			return RedirectToAction("AvailableOrders");
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
