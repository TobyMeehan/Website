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

        Task<string> GetTokenAsync(HttpClient httpClient, string authorizationCode); // TODO: proper di of httpclient

        Task<DiscordUser> AuthenticateAsync(string bearerToken);
    }
}
