using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sklad.Data;
using sklad.Extensions;
using sklad.Models;

namespace sklad.Controllers
{
	[Authorize]
	public class CartController : Controller
	{
		private readonly ApplicationDbContext _db;

		public CartController(ApplicationDbContext db)
		{
			_db = db;
		}

		public IActionResult Index()
		{
			return View();
		}

		public IActionResult Add(int id, int amount)
		{
			Dictionary<int, int> cart = HttpContext.Session.GetObject<Dictionary<int, int>>("cart");
			if(cart == null)
			{
				cart = new Dictionary<int, int>();
			}
			if(cart.ContainsKey(id))
			{
				cart[id] += amount;
			}
			else
			{
				cart.Add(id, amount);
			}
			HttpContext.Session.SetObject("cart", cart);
			return RedirectToAction("Index");
		}

		public IActionResult Remove(int id)
		{
			Dictionary<int, int> cart = HttpContext.Session.GetObject<Dictionary<int, int>>("cart");
			cart.Remove(id);
			HttpContext.Session.SetObject("cart", cart);
			return RedirectToAction("Index");
		}
	}
}