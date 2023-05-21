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
    /// <summary>
    /// Only Administrators are allowed to perform CRUD functionalities on LeaveType objects
    /// No further comments added to default methods in this Controller
    /// </summary>
    public class LeaveTypesController : Controller
    {
        private readonly EMSDbContext _context;
        private  CurrentUser2 _currentUser;

        public LeaveTypesController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // GET: LeaveTypes
        public async Task<IActionResult> Index()
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    
           
              return _context.LeaveTypes != null ? 
                          View(await _context.LeaveTypes.ToListAsync()) :
                          Problem("Entity set 'EMSDbContext.LeaveTypes'  is null.");
        }

        // GET: LeaveTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    

            if (id == null || _context.LeaveTypes == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveType);
        }

        // GET: LeaveTypes/Create
        public IActionResult Create()
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    

            return View();
        }

        // POST: LeaveTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] LeaveType leaveType)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    

            if (ModelState.IsValid)
            {
                _context.Add(leaveType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveType);
        }

        // GET: LeaveTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    

            if (id == null || _context.LeaveTypes == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            return View(leaveType);
        }

        // POST: LeaveTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] LeaveType leaveType)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    

            if (id != leaveType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveTypeExists(leaveType.Id))
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
            return View(leaveType);
        }

        // GET: LeaveTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    

            if (id == null || _context.LeaveTypes == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveType);
        }

        // POST: LeaveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;                    

            if (_context.LeaveTypes == null)
            {
                return Problem("Entity set 'EMSDbContext.LeaveTypes'  is null.");
            }
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType != null)
            {
                _context.LeaveTypes.Remove(leaveType);
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


        private bool LeaveTypeExists(int id)
        {
          return (_context.LeaveTypes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
