namespace TobyMeehan.Com.Builders;

public struct CreateApplicationBuilder
{
    public CreateApplicationBuilder WithAuthor(Id<IUser> value) => this with { Author = value };
    public Id<IUser> Author { get; set; }

    public CreateApplicationBuilder WithName(string value) => this with { Name = value };
    public string Name { get; set; }
}