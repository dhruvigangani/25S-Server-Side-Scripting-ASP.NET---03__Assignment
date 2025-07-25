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
    public class PayStubsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PayStubsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PayStubs
        public async Task<IActionResult> Index()
        {
            return View(await _context.PayStubs.ToListAsync());
        }

        // GET: PayStubs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payStub = await _context.PayStubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payStub == null)
            {
                return NotFound();
            }

            return View(payStub);
        }

        // GET: PayStubs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PayStubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,HoursWorked,HourlyRate,PayDate")] PayStub payStub)
        {
            if (ModelState.IsValid)
            {
                _context.Add(payStub);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(payStub);
        }

        // GET: PayStubs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payStub = await _context.PayStubs.FindAsync(id);
            if (payStub == null)
            {
                return NotFound();
            }
            return View(payStub);
        }

        // POST: PayStubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,HoursWorked,HourlyRate,PayDate")] PayStub payStub)
        {
            if (id != payStub.Id)
            {
                return NotFound();
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
                    if (!PayStubExists(payStub.Id))
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
            return View(payStub);
        }

        // GET: PayStubs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var payStub = await _context.PayStubs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (payStub == null)
            {
                return NotFound();
            }

            return View(payStub);
        }

        // POST: PayStubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var payStub = await _context.PayStubs.FindAsync(id);
            if (payStub != null)
            {
                _context.PayStubs.Remove(payStub);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PayStubExists(int id)
        {
            return _context.PayStubs.Any(e => e.Id == id);
        }
    }
}
