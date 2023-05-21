using System.ComponentModel.DataAnnotations;

namespace MKsEMS.Models
{
    public class Job
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public double Salary { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}
