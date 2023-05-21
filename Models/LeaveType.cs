using System.ComponentModel.DataAnnotations;

namespace emsproject23.Models
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default;
    }
}
