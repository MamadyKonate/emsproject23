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
    public class CredentialsController : Controller
    {
        private readonly EMSDbContext _context;
        private CurrentUser2 _currentUser;

        public CredentialsController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        // GET: Credentials
        /// <summary>
        /// Only Adminstrators have access to the list of all Credentials
        /// Any other user should should only see his/her own credentials
        /// </summary>
        /// <returns>List of Credentials</returns>
        public async Task<IActionResult> Index()
        {
            
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;
                      
            if(_context.Credentials != null)
            { 
                return _currentUser.GetLoggedInUser().IsAdmin ? 
                        View(await _context.Credentials.ToListAsync()) :
                        View(await _context.Credentials.Where(c => c.UserEmail == _currentUser.GetLoggedInUser().Email).ToListAsync());
            }

            return Problem("Entity set 'EMSDbContext.Credentials'  is null.");
        }

        // GET: Credentials/Details/5
        /// <summary>
        /// Any logged in user should be able to view their own crdentials
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (!_currentUser.IsLoggedIn())            
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;
           
            if (id == null || _context.Credentials == null)
            {
                return NotFound();
            }

            var credentials = await _context.Credentials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credentials == null)
            {
                return NotFound();
            }

            return View(credentials);
        }        

        // GET: Credentials/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if (id == null || _context.Credentials == null)
            {
                return NotFound();
            }

            var credentials = await _context.Credentials.FindAsync(id);
            if (credentials == null)
            {
                return NotFound();
            }
            return View(credentials);
        }

        // POST: Credentials/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserEmail,EncPass")] Credentials credentials)
        {
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if (id != credentials.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credentials);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialsExists(credentials.Id))
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
            return View(credentials);
        }
        private bool CredentialsExists(int id)
        {
          return (_context.Credentials?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
