using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EmployeeAdminVital10.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EmployeeAdminVital10.Pages.Admin.EmployeeAdmin
{
    public class UpsertModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public UpsertModel(ApplicationDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public Employee Employee { get; set; }
        public int CurrentPartnerId { get; set; }
        public List<SelectListItem> PartnerChoises { get; set; }

        public async Task<IActionResult> OnGet(int? id)
        {
            CurrentPartnerId = 0;
            //only Employees withought a partner should be partnered with, don't give these as a choise
            PartnerChoises = _db.Employee.OrderBy(e => e.Name.ToLower()).Where(employee => employee.Partner == null && employee.Id != id).Select(item =>
               new SelectListItem
               {
                   Value = item.Id.ToString(),
                   Text = item.Name,
               }
            ).ToList();

            Employee = new Employee();
            if (id == null)
            {
                return Page();
            }

            Employee = await _db.Employee.FirstOrDefaultAsync(employee => employee.Id == id);
            if (Employee == null)
            {
                return NotFound();
            }
            else
            {
                CurrentPartnerId = Employee.PartnerId.HasValue ? Employee.PartnerId.Value : 0;
            }
            return Page();
        }

        private async Task CheckOldPartnerPartnerId()
        {
            if (CurrentPartnerId != 0 && CurrentPartnerId != Employee.PartnerId)
            {
                //the partner is changed and current Employee should be removed as partner
                var EmployeeToUnpartner = await _db.Employee.FindAsync(CurrentPartnerId);
                EmployeeToUnpartner.PartnerId = null;
            }
        }

        private async Task SetPartnerPartnerId()
        {
            await CheckOldPartnerPartnerId();
            {
                //Employee has a partner, this partner needs to get current Employee as partner
                var EmployeeToPartner = await _db.Employee.FindAsync(Employee.PartnerId);
                EmployeeToPartner.PartnerId = Employee.Id;
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                if (Employee.PartnerId == -1)
                {
                    Employee.PartnerId = null;
                    await CheckOldPartnerPartnerId();
                }

                if (Employee.Id == 0)
                {
                    _db.Employee.Add(Employee);
                }
                else
                {
                    _db.Employee.Update(Employee);
                }
                await _db.SaveChangesAsync();

                if (Employee.PartnerId != null)
                {
                    await SetPartnerPartnerId();
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
