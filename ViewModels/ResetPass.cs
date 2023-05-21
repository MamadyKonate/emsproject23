using System.ComponentModel.DataAnnotations;

namespace MKsEMS.ViewModels
{
    public class ResetPass
    {
        [Key]
        public int Id { get; set; }
        public string Email { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ReEnterNewPassword { get; set; }
    }
}
