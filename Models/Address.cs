using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
			if (!string.IsNullOrEmpty(UnitNo))
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

		public string NavigationUrl
		{
			get
			{
				var encodedAddress = System.Web.HttpUtility.UrlEncode(ToString(), Encoding.UTF8);
				return @$"https://www.google.com/maps/dir/?api=1&destination={encodedAddress}&travelmode=driving&dir_action=navigate";
			}
		}

		public static Address CompanyAddress
		{
			get
			{
				var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\settings\\address.json");
				if (File.Exists(path))
				{
					return JsonConvert.DeserializeObject<Address>(File.ReadAllText(path));
				}
				return new Address();
			}

			set
			{
				var address = JsonConvert.SerializeObject(value);
				File.WriteAllText(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\settings\\address.json"), address);
			}
		}
	}
}
