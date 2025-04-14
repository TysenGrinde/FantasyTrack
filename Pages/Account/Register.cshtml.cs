using FantasySports.Models;
using FantasySports.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace FantasySports.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly InMemoryUserContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly TeamService _teamService;

        public RegisterModel(InMemoryUserContext context,
                           UserManager<IdentityUser> userManager,
                           SignInManager<IdentityUser> signInManager,
                            TeamService teamService)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _teamService = teamService;
        }

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = default!;

            [Required]
            [StringLength(100, MinimumLength = 4)]
            public string Password { get; set; } = default!;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user, Input.Password);

                // Add debug logging
                Console.WriteLine($"User creation {(result.Succeeded ? "succeeded" : "failed")}");
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        Console.WriteLine($"Error: {error.Description}");
                    }
                }

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToPage("/MyTeam");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return Page();
        }
    }
}
