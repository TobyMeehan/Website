using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Repositories;

namespace TobyMeehan.Com.AspNetCore.Authentication
{
    public class DiscordOAuthEvents : OAuthEvents
    {
        public override Task TicketReceived(TicketReceivedContext context)
        {
            // insert user

            return base.TicketReceived(context);
        }
    }
}
