namespace TobyMeehan.Com.Api.Services.Icons;

public interface IIconService
{
    Task DownloadAsync(string style, string format, Dictionary<string, string> options, Stream destination,
        CancellationToken cancellationToken = default);
}