using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public LoginFormModel Login { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await _userProcessor.Authenticate(Login.Username, Login.Password))
            {
                User user = _mapper.Map<User>(await _userProcessor.GetUserByUsername(Login.Username));

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username)
                };

                user.Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.Name)));

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties
                {
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6)
                };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return LocalRedirect("/");
            }
            else
            {
                ModelState.AddModelError("Login.Username", "Invalid username and password combination.");
                ModelState.AddModelError("Login.Password", "Invalid username and password combination.");

                return Page();
            }
        }
    }
}