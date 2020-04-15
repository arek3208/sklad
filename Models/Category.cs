using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace sklad.Models
{
	public class Category
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "Nazwa")]
		public string Name { get; set; }

		public List<Item> Items { get; set; }
	}
}
