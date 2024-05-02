using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using She___He_Store.Models;

namespace She___He_Store.Controllers
{
    public class BankcardsController : Controller
    {
        private readonly ModelContext _context;

        public BankcardsController(ModelContext context)
        {
            _context = context;
        }

        // GET: Bankcards
        public async Task<IActionResult> Index()
        {
              return _context.Bankcards != null ? 
                          View(await _context.Bankcards.ToListAsync()) :
                          Problem("Entity set 'ModelContext.Bankcards'  is null.");
        }

        // GET: Bankcards/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null || _context.Bankcards == null)
            {
                return NotFound();
            }

            var bankcard = await _context.Bankcards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankcard == null)
            {
                return NotFound();
            }

            return View(bankcard);
        }

        // GET: Bankcards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Bankcards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CardNumber,Cvv,ExpirationDate,Balance")] Bankcard bankcard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bankcard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bankcard);
        }

        // GET: Bankcards/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null || _context.Bankcards == null)
            {
                return NotFound();
            }

            var bankcard = await _context.Bankcards.FindAsync(id);
            if (bankcard == null)
            {
                return NotFound();
            }
            return View(bankcard);
        }

        // POST: Bankcards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,CardNumber,Cvv,ExpirationDate,Balance")] Bankcard bankcard)
        {
            if (id != bankcard.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bankcard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankcardExists(bankcard.Id))
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
            return View(bankcard);
        }

        // GET: Bankcards/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null || _context.Bankcards == null)
            {
                return NotFound();
            }

            var bankcard = await _context.Bankcards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankcard == null)
            {
                return NotFound();
            }

            return View(bankcard);
        }

        // POST: Bankcards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            if (_context.Bankcards == null)
            {
                return Problem("Entity set 'ModelContext.Bankcards'  is null.");
            }
            var bankcard = await _context.Bankcards.FindAsync(id);
            if (bankcard != null)
            {
                _context.Bankcards.Remove(bankcard);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankcardExists(decimal id)
        {
          return (_context.Bankcards?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
