using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Models;

namespace TobyMeehan.Com.Data.Discord
{
    public class DiscordAuthentication : IDiscordAuthentication
    {
        private readonly DiscordOptions _options;
        private readonly HttpClient _client;

        public DiscordAuthentication(IOptions<DiscordOptions> options, HttpClient client)
        {
            _options = options.Value;

            client.BaseAddress = new Uri("https://discord.com/api");

            _client = client;
        }

        public string AuthorizeUri => $"/oauth2/authorize?client_id={_options.ClientId}&redirect_uri={HttpUtility.UrlEncode(_options.RedirectUri)}&response_type=code&scope=identify";

        private async Task<DiscordToken> TokenAsync(string grantType, string key, string value, ulong id = default)
        {
            var response = await _client.PostAsync("/oauth2/token", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", grantType },
                { key, value },
                { "redirect_uri", _options.RedirectUri },
                { "client_id", _options.ClientId },
                { "client_secret", _options.ClientSecret }
            }));

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to authenticate with Discord.");
            }

            Stream content = await response.Content.ReadAsStreamAsync();
            var token = await JsonSerializer.DeserializeAsync<DiscordToken>(content);

            if (id != default)
                token.DiscordId = id;

            return token;
        }

        public async Task<(DiscordToken, DiscordUser)> LoginAsync(string authorizationCode)
        {
            var token = await TokenAsync("authorization_code", "code", authorizationCode);

            var user = await GetUserAsync(token);
            token.DiscordId = user.Id;

            return (token, user);
        }

        private async Task<DiscordToken> RefreshTokenAsync(DiscordToken token)
        {
            return await TokenAsync("refresh_token", "refresh_token", token.RefreshToken, token.DiscordId);
        }

        private async Task<DiscordUser> GetUserAsync(DiscordToken token)
        {
            if (token.Created + TimeSpan.FromSeconds(token.ExpiresIn) < DateTime.Now)
            {
                throw new Exception("Discord token has expired.");
            }

            await RefreshTokenAsync(token);

            var request = new HttpRequestMessage(HttpMethod.Get, "/users/@me");
            request.Headers.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);

            var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            Stream content = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<DiscordUser>(content);
        }
    }
}
