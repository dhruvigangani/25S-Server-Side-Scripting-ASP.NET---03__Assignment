using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularApplication.Data;
using ShiftSchedularApplication.Models;
using Microsoft.AspNetCore.Authorization;


namespace ShiftSchedularApplication.Controllers
{
    [Authorize] // Applies to ALL actions by default
    public class PunchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PunchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Punches
        [AllowAnonymous] // Let anyone view the list
        public async Task<IActionResult> Index()
        {
            return View(await _context.Punches.ToListAsync());
        }

        // GET: Punches/Details/5
        [AllowAnonymous] // Let anyone view details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var punch = await _context.Punches
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,PunchInTime,PunchOutTime")] Punch punch)
        {
            if (ModelState.IsValid)
            {
                // Set the EmployeeId to the current user's ID
                punch.EmployeeId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                
                if (string.IsNullOrEmpty(punch.EmployeeId))
                {
                    ModelState.AddModelError("", "User not found. Please log in again.");
                    return View(punch);
                }
                
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

            var punch = await _context.Punches.FindAsync(id);
            if (punch == null)
            {
                return NotFound();
            }

            // Ensure user can only edit their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || punch.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            return View(punch);
        }

        // POST: Punches/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,PunchInTime,PunchOutTime")] Punch punch)
        {
            if (id != punch.Id)
            {
                return NotFound();
            }

            // Ensure user can only edit their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || punch.EmployeeId != currentUserId)
            {
                return Forbid();
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

            var punch = await _context.Punches
                .FirstOrDefaultAsync(m => m.Id == id);
            if (punch == null)
            {
                return NotFound();
            }

            // Ensure user can only delete their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || punch.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            return View(punch);
        }

        // POST: Punches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var punch = await _context.Punches.FindAsync(id);
            if (punch != null)
            {
                // Ensure user can only delete their own records
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId) || punch.EmployeeId != currentUserId)
                {
                    return Forbid();
                }

                _context.Punches.Remove(punch);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PunchExists(int id)
        {
            return _context.Punches.Any(e => e.Id == id);
        }
    }
}
