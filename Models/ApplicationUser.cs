using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sklad.Models
{
	public class ApplicationUser : IdentityUser
	{
		[Required]
		[PersonalData]
		[Display(Name = "Imię")]
		public string FirstName { get; set; }

		[Required]
		[PersonalData]
		[Display(Name = "Nazwisko")]
		public string LastName { get; set; }

		[PersonalData]
		[Display(Name = "Firma")]
		public string Company { get; set; }

		public List<Address> Addresses { get; set; }

		[InverseProperty("Client")]
		public List<Order> ClientOrders { get; set; }
		[InverseProperty("Driver")]
		public List<Order> DriverOrders { get; set; }

		public string Formatted
		{
			get => ToString();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if(Company != "")
			{
				sb.Append(Company);
				sb.Append(' ');
			}
			sb.Append(FirstName);
			sb.Append(' ');
			sb.Append(LastName);
			sb.Append(' ');
			return sb.ToString();
		}
	}
}
