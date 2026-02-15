using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CorporateHelpdesk.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string? Text { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        // Powiązanie ze zgłoszeniem
        public int TicketId { get; set; }
        public Ticket? Ticket { get; set; }

        // Powiązanie z autorem (kto napisał komentarz)
        public string? AuthorId { get; set; }
        public IdentityUser? Author { get; set; }
    }
}
