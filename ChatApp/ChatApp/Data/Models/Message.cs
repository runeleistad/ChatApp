using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data.Models
{
    public class Message
    {
        public long Id { get; set; }

        [Required]
        public long RoomId { get; set; }

        public Room? Room { get; set; }

        [Required]
        [StringLength(450)]
        public string? SenderId { get; set; }

        public ApplicationUser? Sender { get; set; }

        [Required]
        public required string Content { get; set; }

        public DateTime SentTime { get; set; } = DateTime.UtcNow;
    }
}
