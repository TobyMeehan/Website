namespace Migrations.Entities;

public class Redirect
{
    public required string Id { get; set; }
    public required string ApplicationId { get; set; }
    public required string Uri { get; set; }
}