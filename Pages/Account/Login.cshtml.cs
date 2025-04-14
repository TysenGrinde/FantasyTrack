using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace FantasySports.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; } = default!;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = default!;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public LoginModel(SignInManager<IdentityUser> signInManager,
                        ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            // Clear the existing external cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl ?? Url.Content("~/MyTeam");
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/MyTeam");

            if (ModelState.IsValid)
            {
                // This now properly awaits the async sign-in operation
                var result = await _signInManager.PasswordSignInAsync(
                    Input.Email,
                    Input.Password,
                    Input.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in");
                    return LocalRedirect(ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = ReturnUrl });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt");
                    return Page();
                }
            }

            return Page();
        }
    }
}