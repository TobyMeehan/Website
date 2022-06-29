namespace TobyMeehan.Com.Builders;

public struct CreateUserRoleBuilder
{
    public CreateUserRoleBuilder WithName(string value) => new() { Name = value };
    public string Name { get; set; }
}