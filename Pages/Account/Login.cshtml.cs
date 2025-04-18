using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using FantasySports.Models;

namespace FantasySports.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;

        [BindProperty]
        public InputModel Input { get; set; } = default!;

        public string? ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "UserName")]
            public string UserName { get; set; } = default!;

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; } = default!;

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public LoginModel(SignInManager<ApplicationUser> signInManager,
                        ILogger<LoginModel> logger)
        {
            _signInManager = signInManager;
            _logger = logger;
        }

        public async Task<IActionResult> OnGetAsync(string? returnUrl = null)
        {
            // Redirect if user is already authenticated
            if (User?.Identity?.IsAuthenticated == true)
            {
                return RedirectToPage("/Index");
            }

            // Clear external cookie (original behavior)
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ReturnUrl = returnUrl ?? Url.Content("~/Index");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/Index");

            if (ModelState.IsValid)
            {
                // This now properly awaits the async sign-in operation
                var result = await _signInManager.PasswordSignInAsync(
                    Input.UserName,
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