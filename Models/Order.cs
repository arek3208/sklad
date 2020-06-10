using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sklad.Models
{
	public class Order
	{
		[Display(Name = "Numer zamówienia")]
		public int Id { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		[Display(Name = "Data zamówienia")]
		public DateTime OrderDate { get; set; } = DateTime.Now;

		[Display(Name = "Data realizacji")]
		public DateTime? RealizationDate { get; set; }

		[ForeignKey("Client")]
		public string ClientId { get; set; }
		[Display(Name = "Klient")]
		public ApplicationUser Client { get; set; }

		[ForeignKey("Driver")]
		public string DriverId { get; set; }
		[Display(Name = "Kierowca")]
		public ApplicationUser Driver { get; set; }

		public int AddressId { get; set; }
		[Display(Name = "Adres")]
		public Address Address { get; set; }

		[Display(Name = "Towary")]
		public List<OrderItem> OrderItems { get; set; }
	}
}
