using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using emsproject23.Data;
using emsproject23.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace emsproject23.Controllers
{
    public class LeavesController : Controller
    {
        private readonly EMSDbContext _context;
        private  CurrentUser2 _currentUser;
        private DateTime _startDate = new(), _endDate = new();
        TimeSpan duration;
        int daysOff;
        LeaveType leaveType = new();

        public LeavesController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }    

        public bool EnoughDaysRemained(string userEmail, string leaveType)
        {      
            User user = new();
            user = _context.Users.Where(u => u.Email == userEmail).First();
            
            double remainingDays = user.LeaveEntitement - user.LeaveTaken;

            duration = _endDate - _startDate;

            daysOff = duration.Days + 1;  //this is needed otherwise requested date range will be off by -1 day, 
                        
            if (leaveType != "Annual Leave") //we are only interested in checking annual leave left for the user
                return true;

            return remainingDays >= daysOff;
        }

        // GET: Leaves1
        public async Task<IActionResult> Index()
        {
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;
                        
            if (_context.Credentials != null)
            {
                return _currentUser.GetLoggedInUser().IsAdmin ?
                        View(await _context.Leaves.ToListAsync()) :

                        _currentUser.GetLoggedInUser().IsManager ?
                        View(await _context.Leaves.Where(l => l.ManagerEmail.Equals(_currentUser.GetLoggedInUser().Email)
                                                      || l.UserEmail.Equals(_currentUser.GetLoggedInUser().Email)
                                                        ).ToListAsync()) :

                        View(await _context.Leaves.Where(l => l.UserEmail.Equals(_currentUser.GetLoggedInUser().Email)).ToListAsync());
            }

            return Problem("Entity set 'EMSDbContext.Leaves'  is null.");
        }

        // GET: Leaves1/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            TempData["LeaveRqMsg"] = "";

            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;
            
            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leave == null)
            {
                return NotFound();
            }

            return View(leave);
        }

        // GET: Leaves1/Create
        public IActionResult Create()
        {
            TempData["LeaveRqMsg"] = "";

            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            return View();
        }

        // POST: Leaves1/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserEmail,Status, ManagerEmail,DateFrom,DateTo,LeaveType")] Leave leave)
        {
            TempData["LeaveRqMsg"] = "";
          
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if (ModelState.IsValid)
            {
                _startDate = leave.DateFrom.ToDateTime(new TimeOnly());
                _endDate = leave.DateTo.ToDateTime(new TimeOnly()); 

                if (EnoughDaysRemained(leave.UserEmail, leave.LeaveType))
                {
                    //Chekcing valid start date 
                    
                    if ((_startDate.CompareTo(DateTime.Now.Date)>= 0 
                        && _endDate.CompareTo(DateTime.Now.Date) >= 0)
                        && _startDate.Date <= _endDate.Date) 
                    {                        
                                             

                        _context.Add(leave);

                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index));  
                    }                   
                   
                    TempData["LeaveRqMsg"] = "Leave start date must not be later than leave end date, and they cannot be in the past!  Please tray again";
                    return View(leave);                    
                }
                else
                {
                    TempData["LeaveRqMsg"] = "You don't seem to have enough days to book.";
                }                
            }
            else
            {
                TempData["LeaveRqMsg"] = "Somthing went wrong, please try again";

                return View(leave);
            }
               
            return View(leave);
        }

        // GET: Leaves1/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            TempData["LeaveRqMsg"] = "";
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }
            return View(leave);
        }

        // POST: Leaves1/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserEmail,ManagerEmail,DateFrom,DateTo,LeaveType, Status,DenialReason")] Leave leave)
        {           
            TempData["LeaveRqMsg"] = "";

            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if(!AdminUserIsLoggedIn())
                return RedirectToAction("Index", "Leaves"); //Only if user is Administrator;

            if (id != leave.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _startDate = leave.DateFrom.ToDateTime(new TimeOnly());
                _endDate = leave.DateTo.ToDateTime(new TimeOnly());

                if (EnoughDaysRemained(leave.UserEmail, leave.LeaveType))
                {
                    //Chekcing valid start date 
                    if (_startDate.CompareTo(DateTime.Now.Date) >= 0
                        && _endDate.CompareTo(DateTime.Now.Date) >= 0
                        && _startDate.Date <= _endDate.Date)
                    {
                        leave.Status = "Pending";
                    }
                    else
                    {
                        TempData["LeaveRqMsg"] = "Leave request failed to be edited. Please check date range is valid";
                        return View(leave);
                    }
                

                    try
                    {
                        _context.Update(leave);
                        await _context.SaveChangesAsync();

                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LeaveExists(leave.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }

                    TempData["LeaveRqMsg"] = "Leave request was successfully edited.";

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["LeaveRqMsg"] = $"Not enough days remained on annual leave for {leave.UserEmail}.";
                }

                TempData["LeaveRqMsg"] = "The Leave Request could not be processed.  Please try again.";
            }            

            return View(leave);
        }

        public async Task<IActionResult> ProcessLeave(int? id)
        {

            TempData["LeaveRqMsg"] = "";
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves.FindAsync(id);
            if (leave == null)
            {
                return NotFound();
            }
            return View(leave);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessLeave(int id, [Bind("Id,UserEmail,ManagerEmail,DateFrom,DateTo,LeaveType, Status,DenialReason")] Leave leave)
        {
            TempData["LeaveRqMsg"] = "";

            User user = new();
            user = _context.Users.Where(u => u.Email == leave.UserEmail).First();

            if (user.Id == _currentUser.GetLoggedInUser().Id)
            {
                TempData["LeaveRqMsg"] = "You cannot process your own Lear Request.  Ask your to process this for you";
                return View(leave);
            }


            _startDate = leave.DateFrom.ToDateTime(new TimeOnly());
            _endDate = leave.DateTo.ToDateTime(new TimeOnly());

            if (EnoughDaysRemained(leave.UserEmail, leave.LeaveType))
            {
                //Chekcing the leave date range validity
                if (_startDate.CompareTo(DateTime.Now.Date) >= 0
                    && _endDate.CompareTo(DateTime.Now.Date) >= 0
                    && _startDate.Date <= _endDate.Date)
                {
                    
                    leave.numberOfDays = daysOff;
                    
                    

                    //Approved will be replaced with Ennum value
                    if (leave.Status == "Approved")
                    {         
                        string annualLeave = _context.LeaveTypes.First().Name; //Annual Leave is the first element in LeaveTypes table

                        if (leave.LeaveType == annualLeave)
                          user.LeaveTaken += daysOff; //only if the leave is an annual leave, we update LeaveTaken in User table                   

                        if (leave.LeaveType == "Sick Leave")
                            user.SickLeaveTaken += daysOff; //and if the leave is an sick leave, we update SickLeaveTaken in User table                   
                        
                        TempData["LeaveRqMsg"] = $"Leave reqest for {_startDate} to {_endDate} has been approved.";
                    }
                    if (leave.Status == "Denied")
                    {                                                
                        TempData["LeaveRqMsg"] = $"Leave reqest for {_startDate} to {_endDate} has been declined.";
                    }
                    
                    try
                    {
                        _context.Update(leave);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!LeaveExists(leave.Id))
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
                
                TempData["LeaveRqMsg"] = "Invalid date range entered.  Please check and try again.";
                return View(leave);
            }
            
            TempData["LeaveRqMsg"] = $"No enough days remained on annual leave for {leave.UserEmail}.  Total days left is {user.LeaveEntitement - user.LeaveTaken}";
            return View(leave);
        }

        // GET: Leaves1/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if (id == null || _context.Leaves == null)
            {
                return NotFound();
            }

            var leave = await _context.Leaves
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leave == null)
            {
                return NotFound();
            }

            return View(leave);
        }

        // POST: Leaves1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!_currentUser.IsLoggedIn())
                return RedirectToAction("Index", "UserLogins"); //Only if user is not already logged in;

            if (_context.Leaves == null)
            {
                return Problem("Entity set 'EMSDbContext.Leaves'  is null.");
            }
            var leave = await _context.Leaves.FindAsync(id);
            if (leave != null)
            {
                _context.Leaves.Remove(leave);
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
                return false; //no point in checking anything else otherwise we get null reference exception

            if (_currentUser.IsLoggedIn() && _currentUser.GetLoggedInUser().IsAdmin)
                return true;

            return false;
        }
        private bool LeaveExists(int id)
        {
          return (_context.Leaves?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
