using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data.Models
{
    public class Room
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public List<RoomUser> RoomUsers { get; set; }

        public List<Message> Messages { get; set; }
    }
}
