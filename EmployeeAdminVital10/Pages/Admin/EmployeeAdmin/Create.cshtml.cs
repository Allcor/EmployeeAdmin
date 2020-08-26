using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeAdminVital10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeAdminVital10.Pages.Admin.EmployeeAdmin
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Employee Employee { get; set; }
        public List<SelectListItem> PartnerChoises { get; set; }

        public void OnGet()
        {
            PartnerChoises = _db.Employee.OrderBy(e => e.Name.ToLower()).Where(employee => employee.Partner == null).Select(item =>
               new SelectListItem
               {
                   Value = item.Id.ToString(),
                   Text = item.Name,
               }
            ).ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (Employee.PartnerId == -1)
                {
                    Employee.PartnerId = null;
                }
                await _db.Employee.AddAsync(Employee);
                await _db.SaveChangesAsync();

                if (Employee.PartnerId > 0)
                {
                    //now we have a Id for current Employee, set it for chosen partner
                    var EmployeeToPartner = await _db.Employee.FindAsync(Employee.PartnerId);
                    EmployeeToPartner.PartnerId = Employee.Id;
                    await _db.SaveChangesAsync();
                }
                return RedirectToPage("Index");
            }
            else
            {
                return Page();
            }
        }
    }
}
