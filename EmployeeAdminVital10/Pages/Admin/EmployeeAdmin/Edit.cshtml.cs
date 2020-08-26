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
                    //partner Employee should get EmployeeToEdit as new partner
                    var EmployeeToPartner = await _db.Employee.FindAsync(Employee.PartnerId);
                    EmployeeToPartner.PartnerId = Employee.Id;
                    if (EmployeeToEdit.PartnerId != null && EmployeeToEdit.PartnerId > 0)
                    {
                        //if EmployeeToEdit had a partner, it will be replaced, remove current from old partner
                        var EmployeeToUnpartner = await _db.Employee.FindAsync(EmployeeToEdit.PartnerId);
                        if (EmployeeToPartner.Id != EmployeeToUnpartner.Id)
                        {
                            EmployeeToUnpartner.PartnerId = null;
                        }
                    }
                }
                else
                {
                    //check if there was a partner and set it to null
                    if (EmployeeToEdit.PartnerId != null && EmployeeToEdit.PartnerId > 0)
                    {
                        //also remove EmployeeToEdit from partner 
                        var EmployeeToUnpartner = await _db.Employee.FindAsync(EmployeeToEdit.PartnerId);
                        EmployeeToUnpartner.PartnerId = null;
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
