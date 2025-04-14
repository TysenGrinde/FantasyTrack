using FantasySports.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

public class InMemoryUserContext
{
    public List<IdentityUser> Users { get; } = new();
    public List<Team> Teams { get; } = new();
}

namespace FantasySports.Data
{
    public class InMemoryUserStore : IUserStore<IdentityUser>,
                                IUserEmailStore<IdentityUser>,
                                IUserPasswordStore<IdentityUser>
    {
        private readonly InMemoryUserContext _context;

        public InMemoryUserStore(InMemoryUserContext context)
        {
            _context = context;
        }

        // Required IUserStore methods
        public Task<IdentityResult> CreateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            _context.Users.Add(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            _context.Users.Remove(user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u => u.Id == userId));
        }

        public Task<IdentityUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u =>
                u.NormalizedUserName == normalizedUserName));
        }

        public Task<string?> GetNormalizedUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetUserIdAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id);
        }

        public Task<string?> GetUserNameAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task SetNormalizedUserNameAsync(IdentityUser user, string? normalizedName, CancellationToken cancellationToken)
        {
            user.NormalizedUserName = normalizedName ?? string.Empty;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(IdentityUser user, string? userName, CancellationToken cancellationToken)
        {
            user.UserName = userName ?? string.Empty;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            if (existingUser != null)
            {
                _context.Users.Remove(existingUser);
                _context.Users.Add(user);
                return Task.FromResult(IdentityResult.Success);
            }
            return Task.FromResult(IdentityResult.Failed());
        }

        // IUserEmailStore methods
        public Task<IdentityUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            return Task.FromResult(_context.Users.FirstOrDefault(u =>
                u.NormalizedEmail == normalizedEmail));
        }

        public Task<string?> GetEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<string?> GetNormalizedEmailAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.NormalizedEmail);
        }

        public Task SetEmailAsync(IdentityUser user, string? email, CancellationToken cancellationToken)
        {
            user.Email = email ?? string.Empty;
            return Task.CompletedTask;
        }

        public Task SetEmailConfirmedAsync(IdentityUser user, bool confirmed, CancellationToken cancellationToken)
        {
            user.EmailConfirmed = confirmed;
            return Task.CompletedTask;
        }

        public Task SetNormalizedEmailAsync(IdentityUser user, string? normalizedEmail, CancellationToken cancellationToken)
        {
            user.NormalizedEmail = normalizedEmail ?? string.Empty;
            return Task.CompletedTask;
        }

        // IUserPasswordStore methods
        public Task<string?> GetPasswordHashAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(IdentityUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(!string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task SetPasswordHashAsync(IdentityUser user, string? passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // No resources to dispose
        }
    }
}
