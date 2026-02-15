using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CorporateHelpdesk.Models;

namespace CorporateHelpdesk.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<CorporateHelpdesk.Models.Ticket> Ticket { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
    }
}
