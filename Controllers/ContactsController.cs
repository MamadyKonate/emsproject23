using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using emsproject23.Data;
using emsproject23.Models;
using emsproject23.ViewModels;

namespace emsproject23.Controllers
{
    public class ContactsController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly User _loggedInUser = new();
        private List<Contact> _contacts = new();
        public ContactsController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _loggedInUser = currentUser.GetLoggedInUser();           
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            if (_loggedInUser == null)
            {
                TempData["AdminMessage"] = "Please login to proceed";
                return RedirectToAction("Index", "UserLogins");
            }
            return _context.Contacts != null ?
                        View( GetRelevantContacts()) :
                        Problem("Entity set 'EMSDbContext.Contacts'  is null.");
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if(_loggedInUser == null)
            {
                TempData["AdminMessage"] = "Please login to proceed";
                return RedirectToAction("Index", "UserLogins");
            }
            
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        //GET: Contacts/Create
        public IActionResult Create()
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AddressLine1,AddressLine2,AddressLine3,City,County,Eircode,Phone,UserEmail")] Contact contact)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "Users");
            }

            if (ModelState.IsValid)
            {
                _context.Add(contact);
                await _context.SaveChangesAsync();
                var user = _context.Users.FirstOrDefault(u => u.Email == contact.UserEmail);

                return RedirectToAction("index", "Users");
            }

            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "UserLogins");
            }

            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AddressLine1,AddressLine2,AddressLine3,City,County,Eircode,Phone,UserEmail")] Contact contact)
        {
            if (!AdminUserIsLoggedIn())
            {
                TempData["AdminMessage"] = "Please login as an Administrator";
                return RedirectToAction("Index", "UserLogins");
            }

            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contact);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            return View(contact);
        }

        //// GET: Contacts/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (!AdminUserIsLoggedIn())
        //    {
        //        TempData["AdminMessage"] = "Please login as an Administrator";
        //        return RedirectToAction("Index", "Users");
        //    }

        //    if (id == null || _context.Contacts == null)
        //    {
        //        return NotFound();
        //    }

        //    var contact = await _context.Contacts
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (contact == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(contact);
        //}

        //// POST: Contacts/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (!AdminUserIsLoggedIn())
        //    {
        //        TempData["AdminMessage"] = "Please login as an Administrator";
        //        return RedirectToAction("Index", "Users");
        //    }
            
        //    if (_context.Contacts == null)
        //    {
        //        return Problem("Entity set 'EMSDbContext.Contacts'  is null.");
        //    }
        //    var contact = await _context.Contacts.FindAsync(id);
        //    if (contact != null)
        //    {
        //        _context.Contacts.Remove(contact);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private List<Contact> GetRelevantContacts()
        {
            _contacts = _loggedInUser.IsAdmin ?
                            _context.Contacts.ToList() :

                        _loggedInUser.IsManager ?
                            GetManagersContacts() :

                        _context.Contacts.Where(c => c.UserEmail .Equals(_loggedInUser.Email)).ToList();
            return _contacts;
        }

        private bool AdminUserIsLoggedIn()
        {
            if (_loggedInUser == null)
                return false;
           
            if (_loggedInUser.IsUserLoggedIn && _loggedInUser.IsAdmin)
                return true;

            return false;
        }

        private List<Contact> GetManagersContacts()
        {

           //Getting all users' emails whose Manager is the logged in user

            List<string> UserEmailssUnaderManager = new();
            
            foreach(var user in  _context.Users.ToList().Where(u => u.ManagerEmail.Equals(_loggedInUser.Email)))
            {
                UserEmailssUnaderManager.Add(user.Email);
            }

            //All contacts
            _contacts = _context.Contacts.ToList();

            //we now retrieve only and all mstching Contacts for the Manager
            List<Contact> result = new();
            result = (from c in _contacts
                        where UserEmailssUnaderManager.Contains(c.UserEmail)
                        select c).ToList();
            List<Contact> currentContact = _contacts.Where(c => c.UserEmail.Equals(_loggedInUser.Email)).ToList();
            
            if (currentContact.Count > 0)            
                result.Add(currentContact.First());
            
            return result;
        }

        private bool ContactExists(int id)
        {
            return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

