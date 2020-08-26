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
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Employee Employee { get; set; }
        public List<SelectListItem> PartnerChoises { get; set; }

        public async Task OnGet(int id)
        {
            Employee = await _db.Employee.FindAsync(id);
            PartnerChoises = _db.Employee.OrderBy(e => e.Name.ToLower()).Where(employee => employee.Partner == null && employee.Id != id).Select(item =>
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
                var EmployeeToEdit = await _db.Employee.FindAsync(Employee.Id);
                if (Employee.PartnerId > 0)
                { 
                    var EmployeeToPartner = await _db.Employee.FindAsync(Employee.PartnerId);
                    EmployeeToPartner.PartnerId = Employee.Id;
                    if (EmployeeToEdit.PartnerId != null && EmployeeToEdit.PartnerId > 0)
                    {
                        var EmployeeToUnpartner = await _db.Employee.FindAsync(EmployeeToEdit.PartnerId);
                        if (EmployeeToPartner.Id != EmployeeToUnpartner.Id)
                        {
                            EmployeeToUnpartner.PartnerId = null;
                        }
                    }
                }
                else
                {
                    if (EmployeeToEdit.PartnerId != null && EmployeeToEdit.PartnerId > 0)
                    {
                        var EmployeeToPartner = await _db.Employee.FindAsync(EmployeeToEdit.PartnerId);
                        EmployeeToPartner.PartnerId = null;
                    }
                    Employee.PartnerId = null;
                }
                EmployeeToEdit.Name = Employee.Name;
                EmployeeToEdit.PartnerId = Employee.PartnerId;

                await _db.SaveChangesAsync();
                return RedirectToPage("Index");
            }
            else
            {
                return Page();
            }
        }
    }
}
