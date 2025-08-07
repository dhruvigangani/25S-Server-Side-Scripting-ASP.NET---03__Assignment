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
    [Authorize]
    public class AvailabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: Availabilities
        public async Task<IActionResult> Index()
        {
            return View(await _context.Availabilities.ToListAsync());
        }

        [AllowAnonymous]
        // GET: Availabilities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var availability = await _context.Availabilities.FirstOrDefaultAsync(m => m.Id == id);
            if (availability == null) return NotFound();

            return View(availability);
        }

        // GET: Availabilities/Create
        public IActionResult Create()
        {
            ViewData["Day"] = new SelectList(System.Enum.GetValues(typeof(System.DayOfWeek)));
            return View();
        }

        // POST: Availabilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,Day,StartAvailability,EndAvailability")] Availability availability)
        {
            if (ModelState.IsValid)
            {
                // Set the EmployeeId to the current user's ID
                availability.EmployeeId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                
                if (string.IsNullOrEmpty(availability.EmployeeId))
                {
                    ModelState.AddModelError("", "User not found. Please log in again.");
                    ViewData["Day"] = new SelectList(System.Enum.GetValues(typeof(System.DayOfWeek)), availability.Day);
                    return View(availability);
                }
                
                _context.Add(availability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Day"] = new SelectList(System.Enum.GetValues(typeof(System.DayOfWeek)), availability.Day);
            return View(availability);
        }

        // GET: Availabilities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var availability = await _context.Availabilities.FindAsync(id);
            if (availability == null) return NotFound();

            // Ensure user can only edit their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || availability.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            ViewData["Day"] = new SelectList(System.Enum.GetValues(typeof(System.DayOfWeek)), availability.Day);
            return View(availability);
        }

        // POST: Availabilities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,Day,StartAvailability,EndAvailability")] Availability availability)
        {
            if (id != availability.Id) return NotFound();

            // Ensure user can only edit their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || availability.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(availability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AvailabilityExists(availability.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Day"] = new SelectList(System.Enum.GetValues(typeof(System.DayOfWeek)), availability.Day);
            return View(availability);
        }

        // GET: Availabilities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var availability = await _context.Availabilities.FirstOrDefaultAsync(m => m.Id == id);
            if (availability == null) return NotFound();

            // Ensure user can only delete their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || availability.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            return View(availability);
        }

        // POST: Availabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availability = await _context.Availabilities.FindAsync(id);
            if (availability != null)
            {
                // Ensure user can only delete their own records
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId) || availability.EmployeeId != currentUserId)
                {
                    return Forbid();
                }

                _context.Availabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AvailabilityExists(int id) => _context.Availabilities.Any(e => e.Id == id);
    }
}