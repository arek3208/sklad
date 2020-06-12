using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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
			if(TempData.ContainsKey("error") && (bool)TempData["error"])
			{
				ModelState.AddModelError(string.Empty, "Wybrany adres jest błędny lub za daleko od składu");
			}
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

			string companyAddress = System.Web.HttpUtility.UrlEncode(Address.CompanyAddress.ToString(), Encoding.UTF8);
			string clientAddress = System.Web.HttpUtility.UrlEncode(Address.ToString(), Encoding.UTF8);
			var path = $@"https://maps.googleapis.com/maps/api/directions/json?origin={companyAddress}&destination={clientAddress}&mode=driving&units=metric&key=AIzaSyA1DV27cYzewZnD4OuKoattJH9KhuIQwLA";
			try
			{
				string json = new WebClient().DownloadString(path);
				var response = JsonConvert.DeserializeObject<MapsAPI.Rootobject>(json);
				if (response.status != "OK" || response.routes[0].legs[0].distance.value > 50000)
				{
					TempData["error"] = true;
					return RedirectToAction("SelectAddress");
				}
			}
			catch(Exception e)
			{
				return new StatusCodeResult(500);
			}

			Dictionary<int, int> cart = HttpContext.Session.GetObject<Dictionary<int, int>>("cart");
			if (cart == null)
			{
				return RedirectToAction("BrowseItems", "Items");
			}
			var user = _userManager.GetUserAsync(User);
			var order = new Order { ClientId = (await user).Id, OrderItems = new List<OrderItem>(), AddressId = AddressId};
			_context.Order.Add(order);
			foreach (var i in cart)
			{
				var item = _context.Item.Find(i.Key);
				if (i.Value < 1 || i.Value > item.Quantity)
				{
					return new ConflictResult();
				}
				item.Quantity -= i.Value;
				var orderitem = new OrderItem { Amount=i.Value, Item=item, Order=order, Price = i.Value * item.Price};
				_context.OrderItem.Add(orderitem);
				order.OrderItems.Add(orderitem);
			}
			_context.SaveChanges();
			HttpContext.Session.Remove("cart");
			return View(order);
		}

		// GET: Orders
		public async Task<IActionResult> MyOrders()
		{
			var user = await _userManager.GetUserAsync(User);
			var orders = _context.Order.Include(o => o.Client).Include(o => o.Driver).Include(o => o.Address).Include(o => o.OrderItems).ThenInclude(o => o.Item).Where(o => o.ClientId == user.Id);
			return View(await orders.ToListAsync());
		}

		[Authorize(Roles = "Admin, Driver")]
		public async Task<IActionResult> AvailableOrders()
		{
			var user = await _userManager.GetUserAsync(User);
			var orders = _context.Order.Include(o => o.Client).Include(o => o.OrderItems).ThenInclude(o => o.Item).Include(o => o.Address);
			var availableOrders = orders.Where(o => o.Driver == null).ToList();
			var myOrders = orders.Where(o => o.DriverId == user.Id && !o.RealizationDate.HasValue).ToList();
			return View(new Tuple<List<Order>, List<Order>>(myOrders, availableOrders));
		}

		[Authorize(Roles = "Admin, Driver")]
		public async Task<IActionResult> DriverFinishedOrders()
		{
			var user = await _userManager.GetUserAsync(User);
			var orders = _context.Order.Include(o => o.Client).Include(o => o.Driver).Include(o => o.Address).Include(o => o.OrderItems).ThenInclude(o => o.Item).Where(o => o.Driver.Id == user.Id && o.RealizationDate.HasValue);
			return View(orders);
		}

		[Authorize(Roles = "Admin, Driver")]
		public async Task<IActionResult> TakeOrder(int id)
		{
			var order = _context.Order.Find(id);
			if(order.Driver != null)
			{
				return Unauthorized();
			}
			order.Driver = await _userManager.GetUserAsync(User);
			_context.Update(order);
			await _context.SaveChangesAsync();
			return RedirectToAction("AvailableOrders");
		}

		[Authorize(Roles = "Admin, Driver")]
		public IActionResult FinishOrder(int id)
		{
			var order = _context.Order.Find(id);
			if (order == null)
			{
				return NotFound();
			}
			if(order.RealizationDate.HasValue)
			{
				return Unauthorized();
			}
			order.RealizationDate = DateTime.Now;
			_context.Update(order);
			_context.SaveChanges();
			return RedirectToAction("AvailableOrders");
		}
	}
}
