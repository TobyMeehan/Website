using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DataAccessLibrary.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;

namespace WebApi.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IUserProcessor _userProcessor;
        private readonly IRoleProcessor _roleProcessor;

        public AuthController(IMapper mapper, IUserProcessor userProcessor, IRoleProcessor roleProcessor)
        {
            _mapper = mapper;
            _userProcessor = userProcessor;
            _roleProcessor = roleProcessor;
        }

        public async Task SignIn(string username)
        {
            UserModel user = _mapper.Map<UserModel>(await _userProcessor.GetUserByUsername(username));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserId)
            };

            user.Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.Name)));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        public async Task<bool> Authenticate(string username, string password)
        {
            if (await _userProcessor.UserExists(username))
            {
                if (await _userProcessor.Authenticate(username, password))
                {
                    await SignIn(username);

                    return true;
                }
            }

            return false;
        }

        [Route("/login")]
        public IActionResult Login([FromQuery]string rediraction = "Index", [FromQuery]string redircontroller = "Home")
        {
            return View();
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login, [FromQuery]string rediraction = "Index", [FromQuery]string redircontroller = "Home")
        {
            if (await Authenticate(login.Username, login.Password))
            {
                return RedirectToAction(rediraction, redircontroller);
            }
            else
            {
                ModelState.AddModelError("Username", "Invalid username and password combination.");
                ModelState.AddModelError("Password", "Invalid username and password combination.");

                login.Password = "";
                return View(login);
            }
        }

        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login");
        }

    }
}