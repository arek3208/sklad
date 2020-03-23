using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sklad.Models
{
	public class CategoryViewModel
	{
		public string Name { get; set; }
		public List<CategoryViewModel> Nodes { get; set; }
	}
}
