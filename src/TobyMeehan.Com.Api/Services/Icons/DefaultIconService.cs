using Microsoft.AspNetCore.Http.Extensions;

namespace TobyMeehan.Com.Api.Services.Icons;

public class DefaultIconService : IIconService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public DefaultIconService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task DownloadAsync(string style, string format, Dictionary<string, string> options, Stream destination, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient();

        var query = new QueryBuilder();

        foreach (var option in options)
        {
            query.Add(option.Key, option.Value);
        }
        
        var response = await client.GetAsync(
            $"https://avatars.tobymeehan.com/7.x/{style}/{format}{query}", cancellationToken);

        response.EnsureSuccessStatusCode();

        await response.Content.CopyToAsync(destination, cancellationToken);
    }
}