using Microsoft.AspNetCore.Identity;

namespace ChatApp.Data.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public List<RoomUser> Rooms { get; set; }

        public List<Message> Messages { get; set; }

        public List<Contact> Contacts { get; set; }

        public List<Contact> Owners { get; set; }
    }

}
