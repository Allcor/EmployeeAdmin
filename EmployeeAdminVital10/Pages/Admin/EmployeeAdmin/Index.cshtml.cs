using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using EmployeeAdminVital10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAdminVital10.Pages.Admin.EmployeeAdmin
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public List<Employee> Employees { get; set; }

        public async Task OnGet()
        {
            Employees = await _db.Employee.OrderBy(e => e.Name.ToLower()).ToListAsync();
        }

        public async Task<IActionResult> OnPostDelete(int id)
        {
            var EmployeeToDelete = await _db.Employee.FindAsync(id);
            if(EmployeeToDelete == null)
            {
                return NotFound();
            }
            if(EmployeeToDelete.PartnerId > 0)
            {
                //remove current Employee as partner, to prevent any key shenanigans also clear own partner
                var EmployeeToPartner = await _db.Employee.FindAsync(EmployeeToDelete.PartnerId);
                EmployeeToPartner.PartnerId = null;
                EmployeeToDelete.PartnerId = null;
            }
            _db.Employee.Remove(EmployeeToDelete);
            await _db.SaveChangesAsync();

            return RedirectToPage("Index");
        }
    }
}
