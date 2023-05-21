using System.ComponentModel.DataAnnotations;

namespace MKsEMS.Models
{
    public  class Company
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public  string Name { get; set; } = "MK's EMS";
        [Required]
        public  string DomainName { get; set; } = "ems.ie";
        
        [Required, EmailAddress]
        public string Email { get; set; } = "company.ems.ie";
        public string LogoURI { get; set; } = "";
        public bool IsToBeDeleted { get; set; } = false;
    }
}
