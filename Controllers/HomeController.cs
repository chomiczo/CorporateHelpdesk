using System.Diagnostics;
using CorporateHelpdesk.Models;
using CorporateHelpdesk.Interfaces; // Musisz to dodaæ!
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity; // I to!

using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CorporateHelpdesk.Data;


namespace CorporateHelpdesk.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITicketService _ticketService; // Nasz "mózg" od zg³oszeñ
        private readonly UserManager<IdentityUser> _userManager; // Do sprawdzania ról
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ITicketService ticketService, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _logger = logger;
            _ticketService = ticketService;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Settings()
        {
            // Pobieramy pierwsze (i jedyne) ustawienie z bazy
            var settings = await _context.Settings.FirstOrDefaultAsync();

            // Jeœli bazy jest pusta, tworzymy domyœlny obiekt
            if (settings == null) settings = new SystemSettings();

            return View(settings);
        }

        public async Task<IActionResult> Index()
        {
            // Jeœli u¿ytkownik jest zalogowany, pobierz mu statystyki
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var isAdmin = User.IsInRole("Admin"); // Sprawdzamy rolê zamiast emaila

                // Pobieramy statystyki z serwisu (stworzyliœmy go w poprzednim kroku)
                var stats = await _ticketService.GetTicketStatsAsync(userId, isAdmin);

                // Przekazujemy dane do widoku przez ViewBag
                ViewBag.Stats = stats;
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SaveSettings(SystemSettings settings)
        {
            if (ModelState.IsValid)
            {
                // 1. Logika zapisu do bazy (u¿ywamy bezpoœrednio _context, jeœli nie masz jeszcze metody w serwisie)
                var dbSettings = await _context.Settings.FirstOrDefaultAsync();
                if (dbSettings == null)
                {
                    _context.Settings.Add(settings);
                }
                else
                {
                    dbSettings.OrganizationName = settings.OrganizationName;
                    dbSettings.EnableComments = settings.EnableComments;
                    dbSettings.DefaultPriority = settings.DefaultPriority;
                    dbSettings.MaxTicketsPerUser = settings.MaxTicketsPerUser;
                    _context.Update(dbSettings);
                }
                await _context.SaveChangesAsync();

                // 2. To musi byæ tutaj - SweetAlert2 to zobaczy
                TempData["Success"] = "Ustawienia systemowe zosta³y zaktualizowane!";

                // 3. To przekierowanie "odœwie¿a" TempData
                return RedirectToAction(nameof(Settings));
            }

            // Jeœli tutaj trafi, to znaczy, ¿e ModelState jest NIEPOPRAWNY
            // Dodajemy b³¹d, ¿ebyœ wiedzia³ co jest nie tak
            TempData["Error"] = "Wyst¹pi³ b³¹d podczas walidacji formularza.";
            return View("Settings", settings);
        }
        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}