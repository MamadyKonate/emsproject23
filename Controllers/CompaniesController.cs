using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using emsproject23.Data;
using emsproject23.Models;
using System.Linq;
using System.Threading.Tasks;

namespace emsproject23.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly CurrentUser2 _currentUser;

        public CompaniesController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }
        

        // GET: Companies
        /// <summary>
        /// Displaying Company details in a list
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            TempData["AdminMessage"] = "";
            if (!_currentUser.IsLoggedIn())
            {
                TempData["Message"] = "";
                TempData["Message"] = "Please login to proceed";
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;            
            }

            if (!GrantedAccess())
            {
                TempData["AdminMessage"] = "Please login as an Administrator, or the CEO";

                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;
            }
            return _context.Companies != null ? 
                          View(await _context.Companies.ToListAsync()) :
                          Problem("Entity set 'EMSDbContext.Companies'  is null.");
        }

        // GET: Companies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            TempData["AdminMessage"] = "";

            if (!GrantedAccess())
            {
                TempData["AdminMessage"] = "Please login as an Administrator, or the CEO";

                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;
            }

            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            if (company == null)
            {
                return NotFound();
            }

            return View(company);
        }

        // GET: Companies/Create
        /// <summary>
        /// Only Administrators can created Company
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            TempData["AdminMessage"] = "";

            if (!_currentUser.GetLoggedInUser().IsAdmin)
            {
                TempData["AdminMessage"] = "Please login as an Administrator";

                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;
            }           

            return View();
        }

        // POST: Companies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Only Administrators are allowed to creat Company
        /// </summary>
        /// <param name="company">Company to be created</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AddressLine1,AddressLine2,City,County,Eircode,Phone,Email,LogoURI,IsToBeDeleted")] Company company)
        {
            TempData["AdminMessage"] = "";

            if (!_currentUser.GetLoggedInUser().IsAdmin)
            {
                TempData["AdminMessage"] = "Please login as an Administrator";

                return RedirectToAction("Index", "UserLogins");
            }

            if (ModelState.IsValid)
            {
                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        /// <summary>
        ///  Only Administrators are allowed to edit Company details
        /// </summary>
        /// <param name="id">Id of selected Company</param>
        /// <returns>Company View</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            TempData["AdminMessage"] = "";

            if (!_currentUser.GetLoggedInUser().IsAdmin)
            {
                TempData["AdminMessage"] = "Please login as an Administrator";

                return RedirectToAction("Index", "UserLogins"); 
            }

            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies.FindAsync(id);
            if (company == null)
            {
                return NotFound();
            }
            return View(company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        ///  Only Administrators are allowed to creat Company
        /// </summary>
        /// <param name="id"></param>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AddressLine1,AddressLine2,City,County,Eircode,Phone,Email,LogoURI,IsToBeDeleted")] Company company)
        {
            TempData["Message"] = "";

            if (!_currentUser.GetLoggedInUser().IsAdmin)
            {
                TempData["Message"] = "Please login as an Administrator";

                return RedirectToAction("Index", "UserLogins");
            }

            if (id != company.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(company);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CompanyExists(company.Id))
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
            return View(company);
        }

        // GET: Companies/Delete/5
        /// <summary>
        ///  Only Administrators, and the CEO are allowed to delete Company
        ///  There always has be at least one active company
        /// </summary>
        /// <param name="id">Id of the selected Company to be deleted</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            TempData["CompanyMsg"] = "";
            if (!GrantedAccess())
                return RedirectToAction("Index", "UserLogins");



            if (id == null || _context.Companies == null)
            {
                return NotFound();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(m => m.Id == id);
            
           int companyCount = _context.Companies.ToList().Where(c => !c.IsToBeDeleted).Count();
            
            if(companyCount < 2 && !company.IsToBeDeleted)
            {
                TempData["CompanyMsg"] = "There in only one Active Company on the system. Please create the Company Details record you whish to use, you can then delete this on.";
                return RedirectToAction(nameof(Index));                
            }

            if (company == null)
            {
            return NotFound();
            }

            return View(company);
        }

        // POST: Companies/Delete/5
        /// <summary>
        /// Only Administrators, and the CEO are allowed to creat Company.
        /// This is done in two folds:
        /// 1 - Administrator deletes the Company, this only marks it as it is to be deleted
        /// 2 - The CEO (second User) also goes through the deletion process, it then gets deleted
        /// </summary>
        /// <param name="id">Id of the selected Company to be deleted</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!GrantedAccess())
                return RedirectToAction("Index", "UserLogings");

            if (_context.Companies == null)
            {
                return Problem("Entity set 'EMSDbContext.Companies'  is null.");
            }
            var company = await _context.Companies.FindAsync(id);
            
            if (company != null)
            {
                if (!company.IsToBeDeleted)
                {
                    company.IsToBeDeleted = true;
                }
                else
                {
                    _context.Companies.Remove(company);
                }                
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CompanyExists(int id)
        {
          return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        /// <summary>
        /// Checks if the current user should have access to Company details
        /// Only CEO and Admins should have access
        /// </summary>
        /// <returns></returns>
        private bool GrantedAccess()
        {
            if (_currentUser.GetLoggedInUser() != null){ //ensure logged in user is not null first before checking its properties
                
                if (_currentUser.GetLoggedInUser().IsCEO || _currentUser.GetLoggedInUser().IsAdmin)
                return true;
            }
                           
            return false;
        }
    }
}
