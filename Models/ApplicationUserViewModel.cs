using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sklad.Models
{
	public class ApplicationUserViewModel
	{
		public ApplicationUser ApplicationUser { get; set; }
		public bool IsAdmin { get; set; }
		public bool IsWarehouseman { get; set; }
		public bool IsDriver { get; set; }
	}
}
