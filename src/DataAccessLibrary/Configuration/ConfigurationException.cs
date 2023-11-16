using Microsoft.Extensions.Configuration;

namespace TobyMeehan.Com.Data.Configuration;

/// <summary>
/// Exception thrown when application configuration is invalid or missing.
/// </summary>
public class ConfigurationException : Exception
{
    public IConfiguration? Configuration { get; }
    public string SectionName { get; }

    public ConfigurationException(IConfiguration? configuration, string sectionName) : base ($"Expected section {sectionName}.")
    {
        Configuration = configuration;
        SectionName = sectionName;
    }
}