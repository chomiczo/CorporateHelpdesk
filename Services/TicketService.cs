using CorporateHelpdesk.Data;
using CorporateHelpdesk.Interfaces;
using CorporateHelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace CorporateHelpdesk.Services
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context) => _context = context;

        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync() =>
            await _context.Ticket.Include(t => t.Comments).ToListAsync();

        public async Task<IEnumerable<Ticket>> GetUserTicketsAsync(string userId) =>
            await _context.Ticket.Where(t => t.OwnerId == userId).ToListAsync();

        public async Task<Ticket> GetTicketDetailsAsync(int id) =>
            await _context.Ticket.Include(t => t.Comments).ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

        public async Task CreateTicketAsync(Ticket ticket, string userId)
        {
            ticket.OwnerId = userId;
            ticket.CreatedAt = DateTime.Now;
            ticket.Status = "Nowe";
            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task AddCommentAsync(int ticketId, string text, string userId)
        {
            var comment = new Comment { TicketId = ticketId, Text = text, AuthorId = userId, CreatedAt = DateTime.Now };
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();
        }

        public async Task ChangeStatusAsync(int ticketId, string newStatus)
        {
            var ticket = await _context.Ticket.FindAsync(ticketId);
            if (ticket != null) { ticket.Status = newStatus; await _context.SaveChangesAsync(); }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTicketAsync(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null)
            {
                _context.Ticket.Remove(ticket);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> TicketExistsAsync(int id)
        {
            return await _context.Ticket.AnyAsync(e => e.Id == id);
        }
    }
}