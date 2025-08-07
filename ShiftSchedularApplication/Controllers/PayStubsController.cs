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
    public class PayStubsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayStubsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index & Details = Allow Anonymous Access
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.PayStubs.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var payStub = await _context.PayStubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payStub == null) return NotFound();

            return View(payStub);
        }

        // All the following actions require login
        [Authorize]
        public IActionResult Create() => View();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,HoursWorked,HourlyRate,PayDate")] PayStub payStub)
        {
            if (ModelState.IsValid)
            {
                // Set the EmployeeId to the current user's ID (as backup)
                payStub.EmployeeId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                
                if (string.IsNullOrEmpty(payStub.EmployeeId))
                {
                    ModelState.AddModelError("", "User not found. Please log in again.");
                    return View(payStub);
                }
                
                _context.Add(payStub);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payStub);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var payStub = await _context.PayStubs.FindAsync(id);
            if (payStub == null) return NotFound();

            // Ensure user can only edit their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || payStub.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            return View(payStub);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,HoursWorked,HourlyRate,PayDate")] PayStub payStub)
        {
            if (id != payStub.Id) return NotFound();

            // Ensure user can only edit their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || payStub.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(payStub);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PayStubExists(payStub.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(payStub);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var payStub = await _context.PayStubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payStub == null) return NotFound();

            // Ensure user can only delete their own records
            var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(currentUserId) || payStub.EmployeeId != currentUserId)
            {
                return Forbid();
            }

            return View(payStub);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payStub = await _context.PayStubs.FindAsync(id);
            if (payStub != null)
            {
                // Ensure user can only delete their own records
                var currentUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId) || payStub.EmployeeId != currentUserId)
                {
                    return Forbid();
                }

                _context.PayStubs.Remove(payStub);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool PayStubExists(int id)
        {
            return _context.PayStubs.Any(e => e.Id == id);
        }
    }
}
