using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using sklad.Data;
using sklad.Models;

namespace sklad.Areas.Identity.Pages.Account.Manage
{
    public class AddressesModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _db;

        public AddressesModel(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public List<Address> Addresses { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            Addresses = await _db.Address.Include(a => a.ApplicationUser).Where(a => a.ApplicationUserId == user.Id).ToListAsync();
            return Page();
        }
    }
}
