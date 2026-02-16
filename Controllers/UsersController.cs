using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CorporateHelpdesk.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = new List<string> { "Admin", "Pracownik" };

            ViewBag.UserName = user.Email;
            ViewBag.UserId = userId;
            ViewBag.UserRoles = userRoles;

            return View(allRoles);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRole(string userId, string roleName, bool addRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            IdentityResult result;

            if (addRole)
            {
                // Sprawdź czy rola w ogóle istnieje w systemie, jeśli nie - stwórz ją (zabezpieczenie)
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                result = await _userManager.AddToRoleAsync(user, roleName);
            }
            else
            {
                result = await _userManager.RemoveFromRoleAsync(user, roleName);
            }

            if (result.Succeeded)
            {
                TempData["Success"] = $"Pomyślnie {(addRole ? "nadano" : "odebrano")} rolę {roleName} użytkownikowi {user.Email}.";
            }
            else
            {
                // Jeśli się nie uda, pokaż błąd z Identity (np. "User already in role")
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                TempData["Error"] = $"Błąd: {errors}";
            }

            return RedirectToAction(nameof(ManageRoles), new { userId = userId });
        }
    }
}