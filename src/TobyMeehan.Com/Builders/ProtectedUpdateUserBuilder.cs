namespace TobyMeehan.Com.Builders;

public struct ProtectedUpdateUserBuilder
{
    public ProtectedUpdateUserBuilder WithUsername(string value) => this with { Username = value };
    public string Username { get; set; }

    public ProtectedUpdateUserBuilder WithPassword(string value) => this with { Password = value };
    public string Password { get; set; }
}