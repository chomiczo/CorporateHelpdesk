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
using CorporateHelpdesk.Interfaces;

namespace CorporateHelpdesk.Controllers
{
    [Authorize] // tylko zalogowani widzą ten kontroler
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService; // Tylko serwis!
        private readonly UserManager<IdentityUser> _userManager; // <--- dodanie user managera w celu identyfikacji tożsamości
        private readonly ApplicationDbContext _context;

        // Wstrzyknięcie bazy i managera użytkowników
        public TicketsController(ITicketService ticketService, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _ticketService = ticketService;
            _userManager = userManager;
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Jeśli jakimś cudem userId jest nullem, nie puszczamy tego dalej
            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            if (User.IsInRole("Admin"))
            {
                return View(await _ticketService.GetAllTicketsAsync());
            }

            // Teraz userId na pewno nie jest nullem, więc ostrzeżenie CS8604 zniknie
            return View(await _ticketService.GetUserTicketsAsync(userId));
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var ticket = await _context.Ticket
                    .Include(t => t.Comments)
                        .ThenInclude(c => c.Author) // <--- BEZ TEGO NIE ZOBACZYSZ MAILA AUTORA
                    .FirstOrDefaultAsync(m => m.Id == id);
            if (ticket == null) return NotFound();

            // Pobieramy ustawienia z bazy
            var settings = await _context.Settings.FirstOrDefaultAsync() ?? new SystemSettings();
            ViewBag.EnableComments = settings.EnableComments;

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create() => View();

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Priority")] Ticket ticket)
        {
            var userId = _userManager.GetUserId(User);
            await _ticketService.CreateTicketAsync(ticket, userId);
            TempData["Success"] = "Zgłoszenie zostało utworzone pomyślnie!";
            return RedirectToAction(nameof(Index));
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var ticket = await _ticketService.GetTicketDetailsAsync(id);
            if (ticket == null) return NotFound();

            // Zabezpieczenie: Tylko właściciel lub Admin może edytować
            var userId = _userManager.GetUserId(User);
            if (ticket.OwnerId != userId && !User.IsInRole("Admin")) return Forbid();

            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Status,Priority,OwnerId,CreatedAt")] Ticket ticket)
        {
            if (id != ticket.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    await _ticketService.UpdateTicketAsync(ticket);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _ticketService.TicketExistsAsync(ticket.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _ticketService.GetTicketDetailsAsync(id);
            if (ticket == null) return NotFound();
            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _ticketService.DeleteTicketAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TicketExists(int id)
        {
            return await _ticketService.TicketExistsAsync(id);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")] // To zabezpiecza metodę na poziomie serwera!
        public async Task<IActionResult> ChangeStatus(int id, string newStatus)
        {
            await _ticketService.ChangeStatusAsync(id, newStatus);
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int ticketId, string commentText)
        {
            var userId = _userManager.GetUserId(User);
            await _ticketService.AddCommentAsync(ticketId, commentText, userId);
            return RedirectToAction("Details", new { id = ticketId });
        }
    }
}
