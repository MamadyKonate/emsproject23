using emsproject23.Models;

namespace emsproject23.ViewModels
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
