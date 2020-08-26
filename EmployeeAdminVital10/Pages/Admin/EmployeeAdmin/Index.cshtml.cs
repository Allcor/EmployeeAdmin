using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using EmployeeAdminVital10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public void OnGet()
        {
            Employees = _db.Employee.OrderBy(e => e.Name.ToLower()).ToList();
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
