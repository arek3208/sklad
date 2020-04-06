using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using sklad.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using sklad.Models;

namespace sklad
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(
					Configuration.GetConnectionString("DefaultConnection")));
			services.AddDefaultIdentity<ApplicationUser>(options => { options.SignIn.RequireConfirmedAccount = true; options.Password.RequireDigit = false; options.Password.RequireLowercase = false; options.Password.RequireNonAlphanumeric = false; options.Password.RequireUppercase = false; })
				.AddRoles<IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>();
			services.AddControllersWithViews();
			services.AddRazorPages();
			services.AddSession();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseSession();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
				endpoints.MapRazorPages();
			});

			

			CreateRoles(serviceProvider).Wait();
		}

		private async Task CreateRoles(IServiceProvider serviceProvider)
		{
			//initializing custom roles 
			var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
			var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
			string[] roleNames = { "Admin", "Warehouseman", "Driver", "Customer"};
			IdentityResult roleResult;

			foreach (var roleName in roleNames)
			{
				var roleExist = await RoleManager.RoleExistsAsync(roleName);
				// ensure that the role does not exist
				if (!roleExist)
				{
					//create the roles and seed them to the database: 
					roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
				}
			}

			// find the user with the admin email 
			var _user = await UserManager.FindByEmailAsync("ADMIN@SKLAD.COM");

			// check if the user exists
			if (_user == null)
			{
				//Here you could create the super admin who will maintain the web app
				var powerUser = new ApplicationUser
				{
					UserName = "admin@sklad.com",
					Email = "admin@sklad.com",
					FirstName = "Admin",
					LastName = "Admin",
					EmailConfirmed = true
				};
				string adminPassword = "1qaz@WSX";

				var createPowerUser = await UserManager.CreateAsync(powerUser, adminPassword);
				if (createPowerUser.Succeeded)
				{
					//here we tie the new user to the role
					await UserManager.AddToRoleAsync(powerUser, "Admin");

				}
				
			}
		}

	}
}
