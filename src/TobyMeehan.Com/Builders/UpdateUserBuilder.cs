namespace TobyMeehan.Com.Builders;

public struct UpdateUserBuilder
{
    public UpdateUserBuilder WithDescription(string? value) => this with { Description = value };
    public Optional<string?> Description { get; set; }

    public UpdateUserBuilder WithName(string value) => this with { Name = value };
    public Optional<string> Name { get; set; }

    public UpdateUserBuilder WithAvatar(FileUploadBuilder value) => this with { Avatar = value };
    public Optional<FileUploadBuilder> Avatar { get; set; }
}