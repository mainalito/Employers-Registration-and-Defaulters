using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CustomUserLogin.Data;
using Microsoft.AspNetCore.Authorization;

namespace CustomUserLogin.Controllers
{
    [Authorize]
    public class EntityRoleTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EntityRoleTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EntityRoleTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.EntityRoleType.ToListAsync());
        }

        // GET: EntityRoleTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityRoleType = await _context.EntityRoleType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entityRoleType == null)
            {
                return NotFound();
            }

            return View(entityRoleType);
        }

        // GET: EntityRoleTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EntityRoleTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] EntityRoleType entityRoleType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(entityRoleType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(entityRoleType);
        }

        // GET: EntityRoleTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityRoleType = await _context.EntityRoleType.FindAsync(id);
            if (entityRoleType == null)
            {
                return NotFound();
            }
            return View(entityRoleType);
        }

        // POST: EntityRoleTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] EntityRoleType entityRoleType)
        {
            if (id != entityRoleType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(entityRoleType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EntityRoleTypeExists(entityRoleType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(entityRoleType);
        }

        // GET: EntityRoleTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var entityRoleType = await _context.EntityRoleType
                .FirstOrDefaultAsync(m => m.Id == id);
            if (entityRoleType == null)
            {
                return NotFound();
            }

            return View(entityRoleType);
        }

        // POST: EntityRoleTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var entityRoleType = await _context.EntityRoleType.FindAsync(id);
            if (entityRoleType != null)
            {
                _context.EntityRoleType.Remove(entityRoleType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EntityRoleTypeExists(int id)
        {
            return _context.EntityRoleType.Any(e => e.Id == id);
        }
    }
}
