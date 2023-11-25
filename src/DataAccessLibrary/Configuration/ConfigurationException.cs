using Microsoft.Extensions.Configuration;

namespace TobyMeehan.Com.Data.Configuration;

/// <summary>
/// Exception thrown when application configuration is invalid or missing.
/// </summary>
public class ConfigurationException<TOptions> : Exception
{
    public TOptions Options { get; }

    public ConfigurationException(TOptions options, string message) : base(message)
    {
        Options = options;
    }
}