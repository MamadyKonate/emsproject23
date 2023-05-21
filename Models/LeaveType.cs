using System.ComponentModel.DataAnnotations;

namespace MKsEMS.Models
{
    public class LeaveType
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = default;
    }
}
