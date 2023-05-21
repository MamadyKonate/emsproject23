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
    public class JobTitlesController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly CurrentUser2 _currentUser;

        public JobTitlesController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // GET: Jobs
        /// <summary>
        /// Displaying a list of all Active Job Titles
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            return _context.Jobs != null ? 
                          View(await _context.Jobs.Where(j => j.IsActive).ToListAsync()) :
                          Problem("Entity set 'EMSDbContext.Jobs'  is null.");
        }

        // GET: Jobs/Details/5
        /// <summary>
        /// Details of a selected Job JobTitle
        /// </summary>
        /// <param name="id">Id of the selected Job JobTitle</param>
        /// <returns>Details of the selected Job JobTitle</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var jobTitle = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobTitle == null)
            {
                return NotFound();
            }

            return View(jobTitle);
        }

        // GET: Jobs/Create
        /// <summary>
        /// Filling out create Job JobTitle form
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            return View();
        }

        // POST: Jobs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Creating the Job JobTitle
        /// </summary>
        /// <param name="jobTitle">Job JobTitle details filled out by Administrator</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,JobTitle,Description")] Job jobTitle)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            if (ModelState.IsValid)
            {
                _context.Add(jobTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(jobTitle);
        }

        // GET: Jobs/Edit/5
        /// <summary>
        /// Filling out the Job JobTitle details on the form
        /// </summary>
        /// <param name="id">Id of the selected Job JobTitle</param>
        /// <returns>JobTitle object</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var jobTitle = await _context.Jobs.FindAsync(id);
            if (jobTitle == null)
            {
                return NotFound();
            }
            return View(jobTitle);
        }

        // POST: Jobs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Editing the filled out Job JobTitle
        /// </summary>
        /// <param name="id">Id of the Job JobTitle to be edited</param>
        /// <param name="jobTitle">JobTitle object of the Job JobTitle to be edited</param>
        /// <returns>JobTitle object</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,JobTitle,Description")] Job jobTitle)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            if (id != jobTitle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jobTitle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobTitleExists(jobTitle.Id))
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
            return View(jobTitle);
        }

        // GET: Jobs/Delete/5
        /// <summary>
        /// Deleting a JobTitle object to be deleted
        /// </summary>
        /// <param name="id">Id of the selected JobTitle</param>
        /// <returns>JobTitle object to be deleted</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            if (id == null || _context.Jobs == null)
            {
                return NotFound();
            }

            var jobTitle = await _context.Jobs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (jobTitle == null)
            {
                return NotFound();
            }

            return View(jobTitle);
        }

        // POST: Jobs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Users");

            if (_context.Jobs == null)
            {
                return Problem("Entity set 'EMSDbContext.Jobs'  is null.");
            }
            var jobTitle = await _context.Jobs.FindAsync(id);
            if (jobTitle != null)
            {
                _context.Jobs.Remove(jobTitle);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Checking if logged in user is an Administrator and logged in
        /// </summary>
        /// <returns></returns>
        private bool AdminUserIsLoggedIn()
        {
            if (_currentUser.GetLoggedInUser() == null)
                return false;


            if (_currentUser.IsLoggedIn() && _currentUser.GetLoggedInUser().IsAdmin)
                return true;

            return false;
        }

        private bool JobTitleExists(int id)
        {
          return (_context.Jobs?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
