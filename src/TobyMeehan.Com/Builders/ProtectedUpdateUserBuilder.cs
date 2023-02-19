namespace TobyMeehan.Com.Builders;

public struct ProtectedUpdateUserBuilder
{
    public ProtectedUpdateUserBuilder WithHandle(string value) => this with { Handle = value };
    public Optional<string> Handle { get; set; }

    public ProtectedUpdateUserBuilder WithPassword(Password value) => this with { Password = value };
    public Optional<Password> Password { get; set; }
}