using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using emsproject23.Data;
using emsproject23.ViewModels;
using emsproject23.Models;
using emsproject23.Services;

namespace emsproject23.Controllers
{
    public class ResetPassesController : Controller
    {
        private readonly EMSDbContext _context;
        private readonly CurrentUser2 _currentUser;

        public ResetPassesController(EMSDbContext context, CurrentUser2 currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        
        public IActionResult ResetPassword()
        {
            TempData["PasswordMsg"] = "";
            return View();
        }

        // POST: ResetPasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// ResetPassword() allows a user to reset his/her password or an Adminstrator to reset Password for a User
        /// Upon receipt of a valid state, and all creteria are met, the password gets encrypted and stored into the database
        /// </summary>
        /// <param name="resetPass">ResetPasses object to be processed</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword([Bind("Id,Email,CurrentPassword,NewPassword,ReEnterNewPassword")] ResetPass resetPass)
        {
            TempData["PasswordMsg"] = "";

            if (ModelState.IsValid)
            {

                //return _context.ResetPasses != null ?
                //          View(await _context.ResetPasses.ToListAsync()) :
                //          Problem("Entity set 'EMSDbContext.ResetPasses'  is null.");

               if( resetPass.NewPassword != resetPass.ReEnterNewPassword)
                {
                    TempData["PasswordMsg"] = "New passwords do not match";

                    return View(resetPass);
                }
                else
                {
                    var credentials =  await _context.Credentials.Where(c => c.UserEmail.ToLower() == resetPass.Email.ToLower()).FirstOrDefaultAsync();
                    
                    if( credentials == null )
                    {
                        TempData["PasswordMsg"] = "Username or password you entered is incorrect";
                        return View(resetPass);
                    }
                    if(EncDecPassword.DecodeFrom64(credentials.EncPass) != resetPass.CurrentPassword)
                    {
                        TempData["PasswordMsg"] = "Username or password you entered is incorrect"; 
                        return View(resetPass);
                    }                        
                    
                    credentials.EncPass = EncDecPassword.Enc64bitsPass(resetPass.NewPassword);
                    await _context.SaveChangesAsync();
                    
                    TempData["PasswordMsg"] = "Password reset successfully";

                    return View();                   
                } 
            }
            return View(resetPass);
        }

        
    }
}
