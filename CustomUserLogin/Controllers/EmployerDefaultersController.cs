using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using CustomUserLogin.Data;
using CustomUserLogin.Models;
using CustomUserLogin.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using CustomUserLogin.Intefaces;

namespace CustomUserLogin.Controllers
{
    [Authorize]
    public class EmployerDefaultersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        private readonly UserManager<ApplicationUser> _userManager;

        public EmployerDefaultersController(
            ApplicationDbContext context,
          IEmailService emailService,
            UserManager<ApplicationUser> userManager
        )
        {
            _context = context;
            _emailService = emailService;
            _userManager = userManager;
        }

        // GET: EmployerDefaulters
        public async Task<IActionResult> Index()
        {
            return View(await _context.EmployerDefaulters.ToListAsync());
        }

        // GET: EmployerDefaulters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employerDefaulter = await _context
                .EmployerDefaulters.Include(ed => ed.employers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employerDefaulter == null)
            {
                return NotFound();
            }

            return View(employerDefaulter);
        }

        // GET: EmployerDefaulters/Create
        public IActionResult Create(int? id)
        {
            if (id == null || !EmployerExists((int)id))
            {
                return NotFound();
            }
            var employerDefaulter = new EmployerDefaulter
            {
                EmployerId =
                    (int)id // Pre-fill EmployerId
                ,
            };
            Console.WriteLine("EmployerId" + " =============>" + employerDefaulter.EmployerId);

            return View(employerDefaulter);
        }

        // POST: EmployerDefaulters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(
                "EmployerId,AffectedEmployees,ContributionMonth,MonthsRun,AmountDefaulted,SurchargeDue,Status"
            )]
                EmployerDefaulter employerDefaulter
        )
        {
            Console.WriteLine("EmployerId" + " =============>" + employerDefaulter.EmployerId);

            if (ModelState.IsValid)
            {
                employerDefaulter.TotalAmountDue = employerDefaulter.AmountDefaulted + employerDefaulter.SurchargeDue;
                employerDefaulter.Status = Enums.EmployerStatus.DemandNoticeSent;
                _context.Add(employerDefaulter);
                await _context.SaveChangesAsync();
                var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                Console.WriteLine("UserEmail ============>" + userEmail);

                var employer = await _context.Employers.FindAsync(employerDefaulter.EmployerId);


                string message = $@"
                        Dear {employer.Name},

                        We hope this email finds you well. We would like to bring to your immediate attention that our records indicate an outstanding balance of 
                        **{employerDefaulter.TotalAmountDue:C}** for the period **{employerDefaulter.ContributionMonth}**.
";

                await _emailService.SendEmailAsync("from@example.com", employer.Email, "Urgent: Employer Payment Default Notice", message);
                TempData["Success"] = "Successfully added as defaulter";
                return RedirectToAction(
                    "Details",
                    "Employers",
                    new { id = employerDefaulter.EmployerId }
                );
            }
            else
            {
                {
                    // Log validation errors to console or debugging
                    foreach (var modelStateKey in ModelState.Keys)
                    {
                        var modelStateVal = ModelState[modelStateKey];
                        foreach (var error in modelStateVal.Errors)
                        {
                            Console.WriteLine(
                                $"Validation Error: {modelStateKey} - {error.ErrorMessage}"
                            );
                        }
                    }
                }

                return View(employerDefaulter);
            }
        }

        // GET: EmployerDefaulters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            // Load from the DB
            var employerDefaulter = await _context.EmployerDefaulters.FindAsync(id);
            if (employerDefaulter == null)
                return NotFound();

            Console.WriteLine(
                "EmployerDefaulter Details ==GET: "
                    + JsonConvert.SerializeObject(employerDefaulter, Formatting.Indented)
            );

            return View(employerDefaulter);
        }

        // POST: EmployerDefaulters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind(
                "Id,EmployerId,AffectedEmployees,ContributionMonth,MonthsRun,AmountDefaulted,SurchargeDue,TotalAmountDue"
            )]
                EmployerDefaulter employerDefaulter
        )
        {
            Console.WriteLine(
                "EmployerDefaulter Details: "
                    + JsonConvert.SerializeObject(employerDefaulter, Formatting.Indented)
            );
            if (id != employerDefaulter.Id)
            {
                Console.WriteLine("Id =========>" + employerDefaulter.Id);

                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employerDefaulter.TotalAmountDue =
                        employerDefaulter.AmountDefaulted + employerDefaulter.SurchargeDue;

                    _context.Update(employerDefaulter);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployerDefaulterExists(employerDefaulter.Id))
                    {
                        Console.WriteLine("Not found");

                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(
                    "Details",
                    "Employers",
                    new { id = employerDefaulter.EmployerId }
                );
            }
            return View(employerDefaulter);
        }

        // GET: EmployerDefaulters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employerDefaulter = await _context.EmployerDefaulters.FirstOrDefaultAsync(m =>
                m.Id == id
            );
            if (employerDefaulter == null)
            {
                return NotFound();
            }

            return View(employerDefaulter);
        }

        // POST: EmployerDefaulters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employerDefaulter = await _context.EmployerDefaulters.FindAsync(id);

            if (employerDefaulter == null)
            {
                return NotFound(); // Prevent errors if record doesn't exist
            }

            int employerId = employerDefaulter.EmployerId; // Store EmployerId before deletion
            _context.EmployerDefaulters.Remove(employerDefaulter);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Employers", new { id = employerId });
        }

        private bool EmployerDefaulterExists(int id)
        {
            return _context.EmployerDefaulters.Any(e => e.Id == id);
        }

        private bool EmployerExists(int id)
        {
            return _context.Employers.Any(e => e.Id == id);
        }
    }
}
