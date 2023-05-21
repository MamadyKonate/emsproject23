using System.ComponentModel.DataAnnotations;

namespace MKsEMS.Models
{
    public class Credentials
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string EncPass { get; set; }        
    }
}
