using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Accounts.Authentication;
using TobyMeehan.Com.Accounts.Models;
using TobyMeehan.Com.Accounts.Models.Authentication.Login;
using TobyMeehan.Com.Accounts.Models.Authentication.Register;
using TobyMeehan.Com.Builders.User;
using TobyMeehan.Com.Services;

namespace TobyMeehan.Com.Accounts.Controllers;

public class AuthenticationController : Controller
{
    private readonly IUserService _users;
    private readonly IAuthenticationService _authentication;

    public AuthenticationController(IUserService users, IAuthenticationService authentication)
    {
        _users = users;
        _authentication = authentication;
    }
    
    [FromQuery] public string ReturnUrl { get; set; } = "/";
    
    [HttpGet("/login")]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        return View(new LoginViewModel
        {
            ReturnUrl = ReturnUrl,
            Form = new LoginFormModel()
        });
    }

    [HttpPost("/login"), ValidateAntiForgeryToken]
    public async Task<IActionResult> LoginAsync(LoginFormModel form, [FromServices] IValidator<LoginFormModel> validator)
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        var validation = await validator.ValidateAsync(form);

        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return View(new LoginViewModel
            {
                ReturnUrl = ReturnUrl,
                Form = new LoginFormModel()
            });
        }

        var result = await _users.GetByCredentialsAsync(form.Username!, form.Password!);

        if (!result.IsSuccess(out var user))
        {
            const string message = "Username or password incorrect.";
            
            ModelState.AddModelError(nameof(form.Username), message);
            ModelState.AddModelError(nameof(form.Password), message);
                
            return View(new LoginViewModel
            {
                ReturnUrl = ReturnUrl,
                Form = new LoginFormModel()
            });
        }

        await _authentication.SignInAsync(user);

        return Redirect(ReturnUrl);
    }
    
    [HttpGet("/register")]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        return View(new RegisterViewModel
        {
            ReturnUrl = ReturnUrl,
            Form = new RegisterFormModel()
        });
    }

    [HttpPost("/register"), ValidateAntiForgeryToken]
    public async Task<IActionResult> RegisterAsync(RegisterFormModel form, [FromServices] IValidator<RegisterFormModel> validator)
    {
        if (User.Identity?.IsAuthenticated is true)
        {
            return Redirect(ReturnUrl);
        }

        var validation = await validator.ValidateAsync(form);

        if (!validation.IsValid)
        {
            validation.AddToModelState(ModelState);
            return View(new RegisterViewModel
            {
                ReturnUrl = ReturnUrl,
                Form = new RegisterFormModel()
            });
        }

        var user = await _users.CreateAsync(new CreateUserBuilder()
            .WithUsername(form.Username!)
            .WithPassword(form.Password!));

        await _authentication.SignInAsync(user);

        return Redirect(ReturnUrl);
    }

    [HttpPost("/register/check-username")]
    public async Task<IActionResult> CheckUsernameAsync(RegisterFormModel form)
    {
        bool valid = await _users.IsUsernameUniqueAsync(form.Username!);
        return new JsonResult(valid);
    }
    
    [HttpGet("/logout")]
    public async Task<IActionResult> LogoutAsync()
    {
        await _authentication.SignOutAsync();

        return Redirect(ReturnUrl);
    }
}