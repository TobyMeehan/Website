using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BlazorUI.Extensions;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BlazorUI
{
    [AutoValidateAntiforgeryToken]
    public class LoginModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly IUserProcessor _userProcessor;

        public LoginModel(IMapper mapper, IUserProcessor userProcessor)
        {
            _mapper = mapper;
            _userProcessor = userProcessor;
        }

        public async Task<IActionResult> OnGet(string ReturnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                // Allows this endpoint to be used to refresh the user's login credentials if they have expired. A component checks for this and redirects if necessary
                await HttpContext.SignOutAsync();

                ReturnUrl ??= "/";
                string userid = HttpContext.User.GetUserId();
                User user = _mapper.Map<User>(await _userProcessor.GetUserById(userid));

                await Login(user);

                return LocalRedirect(ReturnUrl);
            }
            else
            {
                return Page();
            }
        }

        [BindProperty]
        public LoginFormModel LoginForm { get; set; }

        public async Task<IActionResult> OnPost(string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            ReturnUrl ??= "/";

            if (await _userProcessor.Authenticate(LoginForm.Username, LoginForm.Password))
            {
                User user = _mapper.Map<User>(await _userProcessor.GetUserByUsername(LoginForm.Username));

                await Login(user);

                return LocalRedirect(ReturnUrl);
            }
            else
            {
                ModelState.AddModelError("LoginForm.Username", "Invalid username and password combination.");
                ModelState.AddModelError("LoginForm.Password", "Invalid username and password combination.");

                return Page();
            }
        }

        private async Task Login(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim("Username", user.Username)
            };

            foreach (Role role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
        }
    }
}