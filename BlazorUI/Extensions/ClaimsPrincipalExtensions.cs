using AutoMapper;
using BlazorUI.Models;
using DataAccessLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BlazorUI.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static IUserProcessor UserProcessor { get; set; }

        public static IMapper Mapper { get; set; }        

        public static async Task<User> GetUser(this ClaimsPrincipal user)
        {
            return Mapper.Map<User>(await UserProcessor.GetUserByUsername(user.Identity.Name));
        }

        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user.Identity.Name;
        }

        public static string GetUsername(this ClaimsPrincipal user)
        {
            return user.Claims.Where(claim => claim.Type == "Username").ToList().Single().Value;
        }
    }
}
