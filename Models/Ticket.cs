using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CorporateHelpdesk.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Musisz wpisać tytuł!")]
        [Display(Name = "Temat zgłoszenia")]
        public string? Title { get; set; }

        [Required(ErrorMessage = "Opisz swój problem!")]
        [Display(Name = "Opis problemu")]
        public string? Description { get; set; }

        [Display(Name = "Data utworzenia")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Status")]
        public string Status { get; set; } = "Nowe"; // Wartości: Nowe, W toku, Zamknięte

       
        // To połączy zgłoszenie z kontem osoby, która jest zalogowana
        public string? OwnerId { get; set; }
        public IdentityUser? Owner { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();

        [Required]
        public string Priority { get; set; } = "Średni"; // Niski, Średni, Wysoki
    }
}
