using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data.Models
{
    public class RoomUser
    {
        public long Id { get; set; }

        [Required]
        public long RoomId { get; set; }
        
        [Required]
        public Room? Room { get; set; }

        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(450)]
        public string? UserId { get; set; }
    }
}
