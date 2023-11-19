namespace TobyMeehan.Com.Data.Entities;

public class ConfigurationScope : IScope
{
    public required Id<IScope> Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public required string Description { get; set; }
}