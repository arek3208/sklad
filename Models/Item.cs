using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace sklad.Models
{
	public class Item
	{
		public int Id { get; set; }

		[Required]
		[Display(Name = "Nazwa")]
		public string Name { get; set; }

		[Required]
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		[Column(TypeName = "decimal(18,2)")]
		[Range(0, int.MaxValue, ErrorMessage = "Cena nie może być ujemna")]
		[Display(Name = "Cena")]
		public decimal Price { get; set; }

		[Required]
		[Display(Name = "Cena za")]
		public PriceType PriceFor { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna")]
		[Required]
		[Display(Name = "Ilość")]
		public int Quantity { get; set; }

		public int CategoryId { get; set; }
		public Category Category { get; set; }

		public enum PriceType 
		{ 
			[Display(Name = "sztuka")]
			unit, 
			kilogram 
		}

		public string Image { get; set; }
	}
}
