using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using sklad.Models;

namespace sklad.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}
		public DbSet<sklad.Models.OrderItem> OrderItem { get; set; }
		public DbSet<sklad.Models.Address> Address { get; set; }
		public DbSet<sklad.Models.Item> Item { get; set; }
		public DbSet<sklad.Models.Category> Category { get; set; }
		public DbSet<sklad.Models.Order> Order { get; set; }

	}
}
