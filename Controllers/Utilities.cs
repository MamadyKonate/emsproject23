
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Identity.Client;
using emsproject23.Data;
using emsproject23.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace emsproject23.Controllers
{  
    public  class CurrentUser2
    {
        private readonly IHttpContextAccessor _sessionContext;
        private User _loggedInUser { get; set; } = new();
        public CurrentUser2(IHttpContextAccessor sessionContext)
        {
            _sessionContext = sessionContext;
        }

        // NOT SURE ANY LONGER WHY USING SESSION IN THIS AFTER ALL.
        // ESPECIALLY IF INJECTION IS GOING TO BE USED
        // NOT SURE IF IT MIGHT COME HANDY IN THE LONG RUN - REMAINS TO BE SEEN
        
       
        /// <summary>
        /// Converts the _user object to a Json string and stores it in the session.
        /// </summary>
        /// <param name="user">User Currently logged in</param>
        public void SetLoggedInUser(User user)
        {
            int loggedIn = 0; //for storing bool value into DB

            //Session only takes string or int - objects have to be serialized for storing them
            string userJSon = JsonConvert.SerializeObject(user);
            _sessionContext.HttpContext.Session.SetString("_currentUser", userJSon);            
             
            if (GetLoggedInUser().IsUserLoggedIn)
                loggedIn = 1;
                
            _sessionContext.HttpContext.Session.SetString("userEmail", user.Email);               
            _sessionContext.HttpContext.Session.SetInt32("loggedInUserInt", loggedIn);

        }

        /// <summary>
        /// Reconstruct string in Json format from the session back to the User object for processing
        /// If the user is not logged in, a null reference is returned.
        /// </summary>
        /// <returns>Reconstructed logged in User</returns>
        public User GetLoggedInUser ()
        {
            if (_sessionContext.HttpContext.Session.GetString("_currentUser")!= null){

                string userJSon = _sessionContext.HttpContext.Session.GetString("_currentUser");
                _loggedInUser = JsonConvert.DeserializeObject<User>(userJSon);

                return _loggedInUser;
            }
            return null;            
        }

        /// <summary>
        /// 
        /// It will then return true if a _user is logged in.
        /// </summary>
        /// <returns></returns>
        public bool IsLoggedIn()
        {
            if (GetLoggedInUser() == null) 
                return false;          
          
            
            return GetLoggedInUser().IsUserLoggedIn;
        }       
    }

    public static class ViewModelData
    {
        /// <summary>
        /// This is the data that is used to populate the drop down lists.
        /// </summary>
        static AllDropDownListData Alldrop { get; } = new AllDropDownListData(new emsproject23.Data.EMSDbContext());
        /// <summary>
        /// This method returns a list of bookings stored in the Sqlite databe at any given moment.
        /// </summary>
        /// <returns>A List<Booking> of all bookings</returns>
        public static List<Leave>? GetBookings() { return Alldrop.GeLeaves(); }
        
        /// <summary>
        /// This method returns a list of users stored in the Sqlite databe at any given moment.
        /// </summary>
        /// <returns>A list of all users</returns>
        public static List<User>? GetUsers() { return Alldrop.GetUsers(); }
        /// <summary>
        /// The following methods return various data as SelectLists to be displayed in the views.
        /// They are all stored in the Sqlite database.
        /// </summary>
        /// <returns></returns>
        public static SelectList? GetUsersEmailsSelectList() { return new SelectList(Alldrop.GetUsersEmails()); }
        public static SelectList? GetLeaveTypesSelectList() { return new SelectList(Alldrop.GetLeaveTypes()); }
        public static SelectList? GetJobTitlesSelectList() { return new SelectList(Alldrop.GetJobTitles()); }
        public static SelectList? GetUsersManagerEmailsSelectList() { return new SelectList(Alldrop.GetUsersManagerEmails()); }
        public static SelectList? GeGetLeaveAllowancesSelectList() { return new SelectList(Alldrop.GetLeaveAllowances()); }
        public static SelectList? GetLeaveStatusSelectList() { return new SelectList(Alldrop.LeaveStatus()); }
        public static List<string> GetLeaveStatus() { return Alldrop.LeaveStatus(); }
       
    }

    public class AllDropDownListData
    {
        private EMSDbContext _context;

        /// <summary>
        /// Controller setting up the database context for retrieving from
        /// and saving changes to records into the database
        /// </summary>
        /// <param name="context">Database contex</param>
        public AllDropDownListData(EMSDbContext context)
        {
            _context = context;
            GetFilteredUsers = context.Users.ToList();
        }
        /// <summary>
        /// Retrieving all data from the database.
        /// </summary>
        /// <returns>List of User object</returns>
        public List<User>? GetUsers()
        {
            if (_context.Users != null)
                return _context.Users.ToList();
            return null;
        }
        /// <summary>
        /// Retrieving all emails for populating the email field when 
        /// an admin _user is creating/updating User account
        /// </summary>
        /// <returns>List emails belonging to Users</returns>
        public List<string>? GetUsersEmails()
        {
            List<string> _emails = new();

            if (_context.Users != null)
                foreach (var item in _context.Users.ToList())
                {
                    _emails.Add(item.Email);
                }

            return _emails;
        }
        /// <summary>
        /// Retrieving all emails for populating the ManagerEmail field when 
        /// an admin _user is creating/updating User accounts
        /// </summary>
        /// <returns>List emails belonging to Users</returns>
        public List<string>? GetUsersManagerEmails()
        {
            List<string> _emails = new();

            if (_context.Users != null)
            {  
                foreach (var n in _context.Users.Where(u => u.IsManager).ToList())
                {
                    _emails.Add(n.Email);
                }              
            }               

            return _emails;
        }

        /// <summary>
        /// Retrieving all LeaveAllowances for populating the LeaveEntitlement field when 
        /// an admin _user is creating/updating User accounts
        /// </summary>
        /// <returns>List LeaveAllowances</returns>
        public List<string>? GetLeaveAllowances()
        {
            List<string> _leaveAllowance = new();

            if (_context.LeaveAllowances != null)
            {
                foreach (var la in _context.LeaveAllowances)
                {
                    _leaveAllowance.Add(la.Allowance.ToString());
                }
            }

            return _leaveAllowance;
        }

        //*****************************************************************************
        /// <summary>
        /// To be removed - no referrence to it
        /// It's no longer needed
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public int GetIdByEmail(string email)
        {
            var _user = GetUsers().Where(u => u.Email == email).First();
            return _user.Id;
        }
        //*******************************************************************************
        /// <summary>
        /// Retrieving all the all LeaveTypes (name) from the database.
        /// </summary>
        /// <returns>List of string LeaveTypes</returns>
        public List<string>? GetLeaveTypes()
        {
            List<string> _allLeaveTypes = new();
            if (_context.LeaveTypes != null)
                foreach (var n in _context.LeaveTypes)
                {
                    _allLeaveTypes.Add(n.Name);
                }
            return _allLeaveTypes;
        }

        public List<string>? GetJobTitles()
        {
            List<string> _allJobTitles = new();
            if (_context.Jobs != null)
                foreach (var t in _context.Jobs)
                {
                    _allJobTitles.Add(t.JobTitle);
                }
            return _allJobTitles;
        }
       
        /// <summary>
        /// Retrieving from the Sqlite DB, all Leave bookings made from the database.
        /// </summary>
        /// <returns>List Booking objects</returns>
        public List<Leave> GeLeaves()
        {
            if (_context.Leaves != null)
            {
                return _context.Leaves.ToList();
            }

            return null;
        }

        /// <summary>
        /// This method returns a list of specific (filtered) users stored in the Sqlite databe at any given moment.
        /// </summary>
        public  List<User> GetFilteredUsers { get; set; } = new();   
        

        public List<string> LeaveStatus()
        {
            List<string> stat = new();          
            
            stat.Add("Pending");
            stat.Add("Approved");
            stat.Add("Denied");

            return stat;
        }

    }

}
