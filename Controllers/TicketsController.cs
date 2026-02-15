using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CorporateHelpdesk.Data;
using CorporateHelpdesk.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CorporateHelpdesk.Controllers
{
    [Authorize] // tylko zalogowani widzą ten kontroler
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // <--- dodanie user managera w celu identyfikacji tożsamości

        // Wstrzyknięcie bazy i managera użytkowników
        public TicketsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            // 1. Pobieramy aktualnie zalogowanego użytkownika
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Challenge(); // Jeśli niezalogowany, wykop do logowania
            }

            // 2. Sprawdzamy, czy to Admin (PROSTY SPOSÓB: po mailu)
            // Wpisz tu SWÓJ email, którego używasz do testów!
            if (user.Email == "admin@admin.com")
            {
                // Admin widzi WSZYSTKIE zgłoszenia
                var allTickets = await _context.Ticket.ToListAsync();
                return View(allTickets);
            }
            else
            {
                // Zwykły user widzi TYLKO SWOJE (filtrujemy po OwnerId)
                var myTickets = await _context.Ticket
                    .Where(t => t.OwnerId == user.Id)
                    .ToListAsync();
                return View(myTickets);
            }
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // .Include, żeby pobrać zgłoszenie RAZEM z jego komentarzami i ich autorami
            var ticket = await _context.Ticket
                .Include(t => t.Comments)
                    .ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (ticket == null) return NotFound();

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,CreatedAt,Status,OwnerId")] Ticket ticket)
        {
            // 1. Pobierz ID aktualnie zalogowanego użytkownika
            var user = await _userManager.GetUserAsync(User);

            // 2. Ustaw brakujące dane automatycznie
            ticket.OwnerId = user.Id;           // Przypisz autora
            ticket.CreatedAt = DateTime.Now;    // Ustaw datę na "teraz"
            ticket.Status = "Nowe";             // Domyślny status

            // 3. Zapisz w bazie 
            _context.Add(ticket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,CreatedAt,Status,OwnerId")] Ticket ticket)
        {
            if (id != ticket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Users, "Id", "Id", ticket.OwnerId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Ticket
                .Include(t => t.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null)
            {
                _context.Ticket.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Ticket.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            var ticket = await _context.Ticket.FindAsync(id);
            if (ticket != null)
            {
                ticket.Status = newStatus;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string commentText)
        {
            if (!string.IsNullOrEmpty(commentText))
            {
                var user = await _userManager.GetUserAsync(User);
                var comment = new Comment
                {
                    TicketId = ticketId,
                    Text = commentText,
                    AuthorId = user.Id,
                    CreatedAt = DateTime.Now
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", new { id = ticketId });
        }
    }
}
