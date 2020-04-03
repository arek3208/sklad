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
		public string Name { get; set; }

		[Required]
		[DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
		[Column(TypeName = "decimal(18,2)")]
		[Range(0, int.MaxValue, ErrorMessage = "Cena nie może być ujemna")]
		public decimal Price { get; set; }

		[Required]
		public PriceType PriceFor { get; set; }

		[Range(0, int.MaxValue, ErrorMessage = "Ilość nie może być ujemna")]
		[Required]
		public int Quantity { get; set; }

		public int CategoryId { get; set; }
		public Category Category { get; set; }

		public enum PriceType { unit, kilogram }

		public Item(Item item)
		{
			Id = item.Id;
			Name = item.Name;
			Price = item.Price;
			PriceFor = item.PriceFor;
			Quantity = item.Quantity;
			Category = item.Category;
			CategoryId = item.CategoryId;
		}
	}
}
