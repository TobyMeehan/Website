using AspNet.Security.OAuth.Discord;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.AspNetCore.Extensions;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.AspNetCore.Authentication
{
    public class DiscordOAuthEvents : OAuthEvents
    {
        private readonly IUserRepository _users;

        public DiscordOAuthEvents(IUserRepository users)
        {
            _users = users;
        }

        public override async Task TicketReceived(TicketReceivedContext context)
        {
            ulong discordId = ulong.Parse(context.Principal.Id());

            var user = await _users.GetByDiscordIdAsync(discordId);

            bool isAuthenticated = context.Properties.Items.TryGetValue("UserId", out string userId);

            // User is not signed in and wants to log in with a new discord account
            if (!isAuthenticated && user is null)
            {
                user = await _users.AddAsync(
                    discordId,
                    context.Principal.Username(),
                    context.Principal.FindFirst(DiscordAuthenticationConstants.Claims.AvatarUrl).Value);
            }

            // User is signed in and wants to connect a new discord account
            else if (isAuthenticated && user is null) 
            {
                user = await _users.GetByIdAsync(userId);

                await _users.AddDiscordAccountAsync(user.Id, discordId);
            }

            // User is signed in and wants to connect an existing discord account
            else if (isAuthenticated && !(user is null)) 
            {
                throw new Exception("Discord account already connected.");
            }

            //
            // User wants to sign back in using their discord account
            //

            context.Principal = new ClaimsPrincipleBuilder()
                    .WithClaims(
                        new Claim(ClaimTypes.NameIdentifier, user.Id),
                        new Claim(ClaimTypes.Name, user.Username))
                    .WithClaims(
                        user.Roles.Select(role =>
                            new Claim(ClaimTypes.Role, role.Name)))
                    .WithAuthenticationScheme(CookieAuthenticationDefaults.AuthenticationScheme)
                    .Build();

            await base.TicketReceived(context);
        }
    }
}
