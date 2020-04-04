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
    public class RegisterModel : PageModel
    {
        private readonly IMapper _mapper;
        private readonly IUserProcessor _userProcessor;

        public RegisterModel(IMapper mapper, IUserProcessor userProcessor)
        {
            _mapper = mapper;
            _userProcessor = userProcessor;
        }
        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public RegisterFormModel Register { get; set; }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (await _userProcessor.UserExists(Register.Username))
            {
                ModelState.AddModelError("Register.Username", "That username already exists.");
                return Page();
            }

            var inputUser = new DataAccessLibrary.Models.User
            {
                Username = Register.Username,
                Email = "",
                Roles = new List<DataAccessLibrary.Models.Role>()
            };

            User user = _mapper.Map<User>(await _userProcessor.CreateUser(inputUser, Register.Password));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Id),
                new Claim("Username", user.Username)
            };

            user.Roles.ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role.Name)));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(6),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return LocalRedirect("/");
        }
    }
}