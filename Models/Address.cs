using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sklad.Models
{
	public class Address
	{
		public int Id { get; set; }

		[Display(Name = "Ulica")]
		public string Street { get; set; }

		[Required]
		[Display(Name = "Numer budynku")]
		public string BuildingNo { get; set; }

		[Display(Name = "Numer lokalu")]
		public string UnitNo { get; set; }

		[Required]
		[Display(Name = "Kod pocztowy")]
		public string PostalCode { get; set; }

		[Required]
		[Display(Name = "Miasto")]
		public string City { get; set; }

		public string ApplicationUserId { get; set; }
		public ApplicationUser ApplicationUser { get; set; }

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder($"{Street} {BuildingNo}");
			if (UnitNo != "")
			{
				sb.Append($"/{UnitNo}");
			}
			sb.Append($" {PostalCode} {City}");
			return sb.ToString();
		}

		public string Formatted
		{
			get => ToString();
		}

	}
}
