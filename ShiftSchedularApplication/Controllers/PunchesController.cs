using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Data;
using ShiftScheduler.Models;

namespace ShiftSchedularApplication.Controllers
{
    public class PunchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PunchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Punches
        public async Task<IActionResult> Index()
        {
            return View(await _context.ErrorViewModels.ToListAsync());
        }

        // GET: Punches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var punch = await _context.ErrorViewModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (punch == null)
            {
                return NotFound();
            }

            return View(punch);
        }

        // GET: Punches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Punches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,PunchInTime,PunchOutTime")] Punch punch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(punch);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(punch);
        }

        // GET: Punches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var punch = await _context.ErrorViewModels.FindAsync(id);
            if (punch == null)
            {
                return NotFound();
            }
            return View(punch);
        }

        // POST: Punches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,PunchInTime,PunchOutTime")] Punch punch)
        {
            if (id != punch.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(punch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PunchExists(punch.Id))
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
            return View(punch);
        }

        // GET: Punches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var punch = await _context.ErrorViewModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (punch == null)
            {
                return NotFound();
            }

            return View(punch);
        }

        // POST: Punches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var punch = await _context.ErrorViewModels.FindAsync(id);
            if (punch != null)
            {
                _context.ErrorViewModels.Remove(punch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PunchExists(int id)
        {
            return _context.ErrorViewModels.Any(e => e.Id == id);
        }
    }
}
