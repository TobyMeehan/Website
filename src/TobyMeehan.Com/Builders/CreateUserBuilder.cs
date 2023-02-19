namespace TobyMeehan.Com.Builders;

public struct CreateUserBuilder
{
    public CreateUserBuilder WithUsername(string value) => this with { Username = value };
    public string Username { get; set; }
    
    public CreateUserBuilder WithPassword(Password value) => this with { Password = value };
    public Password Password { get; set; }
}