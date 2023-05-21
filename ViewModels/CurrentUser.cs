using MKsEMS.Models;

namespace MKsEMS.ViewModels
{
    public class CurrentUser
    {
        public User GetLoggedInUser { get; set; } = new();
        public bool IsLoggedIn()
        {
            return GetLoggedInUser.IsUserLoggedIn;
                
        }
    }
}
