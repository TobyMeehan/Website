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
using TobyMeehan.Com.Data.Authentication;
using TobyMeehan.Com.Data.Extensions;
using TobyMeehan.Com.Data.Models;
using TobyMeehan.Com.Extensions;
using TobyMeehan.Com.Models;

namespace TobyMeehan.Com.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthentication<User> _authentication;
        private readonly IRepository<User> _users;

        public AuthController(IAuthentication<User> authentication, IRepository<User> users)
        {
            _authentication = authentication;
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
                return LocalRedirect(ReturnUrl);
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
                return LocalRedirect(ReturnUrl);
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            ReturnUrl ??= "/";

            var result = await _authentication.CheckPasswordAsync(login.Username, login.Password);

            if (result.Success)
            {
                await SignIn(result.Data);

                return LocalRedirect(ReturnUrl);
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
                return LocalRedirect(ReturnUrl);
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
                return LocalRedirect(ReturnUrl);
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            if ((await _users.GetByAsync(x => x.Username == register.Username)).Any())
            {
                ModelState.AddModelError(nameof(register.Username), "That username already exists.");
                return View();
            }

            await _users.AddAsync(register.Username, register.Password);

            var user = (await _users.GetByAsync(x => x.Username == register.Username)).Single();

            await SignIn(user);

            return LocalRedirect(ReturnUrl);
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout(string ReturnUrl)
        {
            ReturnUrl ??= "/";

            await HttpContext.SignOutAsync();

            return LocalRedirect(ReturnUrl);
        }

        [Route("/refresh")]
        public async Task<IActionResult> Refresh(string ReturnUrl)
        {
            ReturnUrl ??= "/";

            User user = await _users.GetByIdAsync(User.Id());

            await HttpContext.SignOutAsync();

            await SignIn(user);

            return LocalRedirect(ReturnUrl);
        }
    }
}