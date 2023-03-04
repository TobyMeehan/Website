using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TobyMeehan.Com.Accounts.Services;

namespace TobyMeehan.Com.Accounts.Pages;

public class Logout : PageModel
{
    private readonly IAuthenticationService _authentication;

    public Logout(IAuthenticationService authentication)
    {
        _authentication = authentication;
    }
    
    public async Task<IActionResult> OnGetAsync([FromQuery(Name = "ReturnUrl")] string returnUrl = "/")
    {
        await _authentication.SignOutAsync();

        return Redirect(returnUrl);
    }
}