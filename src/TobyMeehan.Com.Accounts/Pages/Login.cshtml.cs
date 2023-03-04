using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TobyMeehan.Com.Accounts.Extensions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Services;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Pages;

public class Login : PageModel
{
    private readonly IValidator<LoginFormModel> _validator;
    private readonly IUserService _users;
    private readonly IAuthenticationService _authentication;

    public Login(IValidator<LoginFormModel> validator, IUserService users, IAuthenticationService authentication)
    {
        _validator = validator;
        _users = users;
        _authentication = authentication;
    }
    
    public IActionResult OnGet([FromQuery(Name = "ReturnUrl")] string returnUrl = "/")
    {
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            return Redirect(returnUrl);
        }
        
        return Page();
    }

    [BindProperty] public LoginFormModel Form { get; set; } = new();

    public async Task<IActionResult> OnPostAsync([FromQuery(Name = "ReturnUrl")] string returnUrl = "/")
    {
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            return Redirect(returnUrl);
        }
        
        var validation = await _validator.ValidateAsync(Form);

        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return Page();
        }

        var user = await _users.GetByCredentialsAsync(Form.Handle!, Form.Password!);

        if (user is null)
        {
            const string message = "Username or password incorrect.";
            
            ModelState.AddModelError(nameof(Form.Handle), message);
            ModelState.AddModelError(nameof(Form.Password), message);

            return Page();
        }
        
        await _authentication.SignInAsync(user);

        return Redirect(returnUrl);
    }
}