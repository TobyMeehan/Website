using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using TobyMeehan.Com.Data;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Data.Repositories;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Controllers
{
    public class AuthController : Controller
    {
        private readonly IUserRepository _users;

        public AuthController(IUserRepository users)
        {
            _users = users;
        }

        private Task SignIn(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username)
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6),
                IsPersistent = true
            };

            return HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
        }

        [Route("/login")]
        public IActionResult Login(string ReturnUrl)
        {
            ReturnUrl ??= "/";

            if (User.Identity.IsAuthenticated)
            {
                return Redirect(ReturnUrl);
            }

            return View();
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login(string ReturnUrl, LoginViewModel login)
        {
            ReturnUrl ??= "/";

            if (User.Identity.IsAuthenticated)
            {
                return Redirect(ReturnUrl);
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            ReturnUrl ??= "/";

            var result = await _users.AuthenticateAsync(login.Username, login.Password);

            if (result.Success)
            {
                await SignIn(result.Result);

                return Redirect(ReturnUrl);
            }
            else
            {
                ModelState.AddModelError(nameof(login.Username), "Invalid username and password combination.");

                ModelState.AddModelError(nameof(login.Password), "Invalid username and password combination.");

                return View();
            }
        }

        [Route("/register")]
        public IActionResult Register(string ReturnUrl)
        {
            ReturnUrl ??= "/";

            if (User.Identity.IsAuthenticated)
            {
                return Redirect(ReturnUrl);
            }

            return View();
        }

        [Route("/register")]
        [HttpPost]
        public async Task<IActionResult> Register(string ReturnUrl, RegisterViewModel register)
        {
            ReturnUrl ??= "/";

            if (User.Identity.IsAuthenticated)
            {
                return Redirect(ReturnUrl);
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if (await _users.AnyUsernameAsync(register.Username))
            {
                ModelState.AddModelError(nameof(register.Username), "That username already exists.");
                return View();
            }

            await _users.AddAsync(register.Username, register.Password);

            var user = await _users.GetByUsernameAsync(register.Username);

            await SignIn(user);

            return Redirect(ReturnUrl);
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout(string ReturnUrl)
        {
            ReturnUrl ??= "/";

            await HttpContext.SignOutAsync();

            return Redirect(ReturnUrl);
        }

        [Route("/refresh")]
        public async Task<IActionResult> Refresh(string ReturnUrl)
        {
            ReturnUrl ??= "/";

            User user = await _users.GetByIdAsync(User.Id());

            await HttpContext.SignOutAsync();

            await SignIn(user);

            return Redirect(ReturnUrl);
        }
    }
}