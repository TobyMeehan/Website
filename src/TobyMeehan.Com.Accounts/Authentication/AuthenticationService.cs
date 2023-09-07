using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using TobyMeehan.Com.Accounts.Extensions;

namespace TobyMeehan.Com.Accounts.Authentication;

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
            throw new Exception("Could not access HttpContext.");
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
            throw new Exception("Could not access HttpContext.");
        }

        await _context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public Id<IUser>? UserId => _context?.User.Id();
}