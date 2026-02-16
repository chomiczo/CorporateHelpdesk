using CorporateHelpdesk.Models;

namespace CorporateHelpdesk.Interfaces
{
    public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<IEnumerable<Ticket>> GetUserTicketsAsync(string userId);
        Task<Ticket> GetTicketDetailsAsync(int id);
        Task CreateTicketAsync(Ticket ticket, string userId);
        Task AddCommentAsync(int ticketId, string text, string userId);
        Task ChangeStatusAsync(int ticketId, string newStatus);

        Task UpdateTicketAsync(Ticket ticket);
        Task DeleteTicketAsync(int id);
        Task<bool> TicketExistsAsync(int id);

        Task<Dictionary<string, int>> GetTicketStatsAsync(string userId, bool isAdmin);
    }
}