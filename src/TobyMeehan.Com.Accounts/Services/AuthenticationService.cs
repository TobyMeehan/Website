using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TobyMeehan.Com.Accounts.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly HttpContext? _context;

    public AuthenticationService(IHttpContextAccessor httpContextAccessor)
    {
        _context = httpContextAccessor.HttpContext;
    }
    
    public async Task SignInAsync(IUser user)
    {
        if (_context is null)
        {
            return;
        }
        
        var identity = new ClaimsIdentity(new[] {new Claim(ClaimTypes.NameIdentifier, user.Id.Value)},
            CookieAuthenticationDefaults.AuthenticationScheme);

        var properties = new AuthenticationProperties
        {
            ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6),
            IsPersistent = true
        };

        await _context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);
    }

    public async Task SignOutAsync()
    {
        if (_context is null)
        {
            return;
        }

        await _context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}