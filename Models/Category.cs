using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


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
