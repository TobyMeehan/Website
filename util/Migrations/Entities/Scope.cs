namespace Migrations.Entities;

public class Scope
{
    public required string Id { get; set; }
    public required string Alias { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public required string Description { get; set; }
}