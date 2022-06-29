namespace TobyMeehan.Com.Builders;

public struct UpdateDownloadBuilder
{
    public Optional<string> Title { get; set; }
    public Optional<string> Summary { get; set; }
    public Optional<string> Description { get; set; }
    public Optional<Visibility> Visibility { get; set; }
    public Optional<Version> Version { get; set; }
}