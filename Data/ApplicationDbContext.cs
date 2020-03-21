using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sklad.Models;

namespace sklad.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<Category>()
				.HasMany(oc => oc.ChildCategories)
				.WithOne(c => c.ParentCategory)
				.HasForeignKey(c => c.CategoryId);
		}

		public DbSet<sklad.Models.OrderItem> OrderItem { get; set; }
		public DbSet<sklad.Models.Address> Address { get; set; }
		public DbSet<sklad.Models.Item> Item { get; set; }
		public DbSet<sklad.Models.Category> Category { get; set; }

		public void DeleteCategory(Category category)
		{
			var target = Category
				.Include(x => x.ChildCategories)
				.FirstOrDefault(x => x.Id == category.Id);
			RecursiveDelete(target);
			SaveChanges();
		}

		private void RecursiveDelete(Category parent)
		{
			if (parent.ChildCategories != null)
			{
				var children = Category
					.Include(x => x.ChildCategories)
					.Where(x => x.CategoryId == parent.Id);
					
				foreach(var child in children)
				{
					RecursiveDelete(child);
				}
			}
			Category.Remove(parent);
		}
	}
}
