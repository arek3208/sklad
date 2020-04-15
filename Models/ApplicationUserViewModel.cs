using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sklad.Models
{
	public class ApplicationUserViewModel
	{
		public ApplicationUser ApplicationUser { get; set; }

		[Display(Name = "Administrator")]
		public bool IsAdmin { get; set; }

		[Display(Name = "Magazynier")]
		public bool IsWarehouseman { get; set; }

		[Display(Name = "Kierowca")]
		public bool IsDriver { get; set; }
	}
}
