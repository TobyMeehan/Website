using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TobyMeehan.Com.Accounts.Authentication;
using TobyMeehan.Com.Accounts.Extensions;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Builders;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Pages;

public class Register : PageModel
{
    private readonly IValidator<RegisterFormModel> _validator;
    private readonly IUserService _users;
    private readonly IAuthenticationService _authentication;

    public Register(IValidator<RegisterFormModel> validator, IUserService users, IAuthenticationService authentication)
    {
        _validator = validator;
        _users = users;
        _authentication = authentication;
    }

    [FromQuery]
    public string ReturnUrl { get; set; } = "/";
    
    public IActionResult OnGet()
    {
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            return Redirect(ReturnUrl);
        }
        
        return Page();
    }
    
    [BindProperty] public RegisterFormModel Form { get; set; } = new();

    public async Task<IActionResult> OnPostAsync()
    {
        if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        {
            return Redirect(ReturnUrl);
        }
        
        var validation = await _validator.ValidateAsync(Form);

        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState, nameof(Form));
            return Page();
        }

        var user = await _users.CreateAsync(new CreateUserBuilder()
            .WithUsername(Form.Username!)
            .WithPassword(Form.Password!));

        await _authentication.SignInAsync(user);

        return Redirect(ReturnUrl);
    }

    public async Task<IActionResult> OnPostCheckUsernameAsync()
    {
        bool valid = await _users.IsHandleUniqueAsync(Form.Username!);
        return new JsonResult(valid);
    }
}