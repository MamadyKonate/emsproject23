using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using emsproject23.Data;
using emsproject23.Models;

namespace emsproject23.Controllers
{
    public class JobsController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly CurrentUser2 _loggedInUser;


        public JobsController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _loggedInUser = currentUser;
        }

        // GET: Jobs
        public async Task<IActionResult> Index()
        {
            if (!_loggedInUser.IsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "UserLogins");
            }

            return _context.Jobs != null ? 
                          View(await _context.Jobs.ToListAsync()) :
                          Problem("Entity set 'EMSDbContext.Jobs'  is null.");
        }

        // GET: Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!_loggedInUser.IsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "UserLogins");
            }

            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // GET: Jobs/Create
        public IActionResult Create()
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobTitle,Salary,Description,IsActive")] Job job)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(job);
        }

        // GET: Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            return View(job);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobTitle,Salary,Description,IsActive")] Job job)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            return View(job);
        }

        // GET: Jobs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            if (_context.Jobs == null)
            {
                return Problem("Entity set 'EMSDbContext.Jobs'  is null.");
            }
            var job = await _context.Jobs.FindAsync(id);
            if (job != null)
            {
                job.IsActive = false;
               //  _context.Jobs.Remove(job);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminUserIsLoggedIn()
        {
            if (_loggedInUser.GetLoggedInUser() == null)
                return false;


            if (_loggedInUser.IsLoggedIn() && _loggedInUser.GetLoggedInUser().IsAdmin)
                return true;

            return false;
        }
        private bool JobExists(int id)
        {
          return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
