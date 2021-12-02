using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Discord
{
    public interface IDiscordAuthentication
    {
        string AuthorizeUri { get; }

        Task<(DiscordToken, DiscordUser)> LoginAsync(string authorizationCode);
    }
}
