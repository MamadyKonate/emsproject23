using System;
using System.ComponentModel.DataAnnotations;

namespace MKsEMS.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(40, MinimumLength = 2)]
        public string Surname { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required, EmailAddress]
        public string ManagerEmail { get; set; }
        [Required, DataType(DataType.Date)]       
        public DateOnly DOB { get; set; }
        public double LeaveEntitement { get; set; } = 25;
        public double LeaveTaken { get; set; }
        public double SickLeaveTaken { get; set; }        
        public bool IsUserLoggedIn { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
        public bool IsCEO { get; set; }
        public bool FirstTimeLogin { get; set; }
    }
}
