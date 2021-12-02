using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Discord
{
    public class DiscordAuthentication : IDiscordAuthentication
    {
        private readonly DiscordOptions _options;

        public DiscordAuthentication(IOptions<DiscordOptions> options)
        {
            _options = options.Value;
        }

        public string AuthorizeUri => $"https://discord.com/api/oauth2/authorize?client_id={_options.ClientId}&redirect_uri={HttpUtility.UrlEncode(_options.RedirectUri)}&response_type=code&scope=identify";

        public async Task<DiscordUser> AuthenticateAsync(string bearerToken)
        {
            using (DiscordSocketClient client = new DiscordSocketClient())
            {
                await client.LoginAsync(TokenType.Bearer, bearerToken);

                return new DiscordUser
                {
                    Id = client.CurrentUser.Id,
                    Username = client.CurrentUser.Username,
                    Discriminator = client.CurrentUser.DiscriminatorValue,
                    AvatarUrl = client.CurrentUser.GetAvatarUrl()
                };
            }
        }

        public async Task<string> GetTokenAsync(HttpClient httpClient, string authorizationCode)
        {
            var response = await httpClient.PostAsync("https://discord.com/api/oauth2/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", authorizationCode },
                { "redirect_uri", _options.RedirectUri },
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret }
            }));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to authenticate with Discord.");
            }

            string content = await response.Content.ReadAsStringAsync();
            string token = JsonConvert.DeserializeObject<dynamic>(content).Token;

            return token;
        }
    }
}
