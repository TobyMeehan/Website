namespace TobyMeehan.Com.Builders;

public struct CreateUserBuilder
{
    public CreateUserBuilder WithUsername(string value) => this with { Username = value };
    public string Username { get; set; }
    
    public CreateUserBuilder WithPassword(string value) => this with { Password = value };
    public string Password { get; set; }
}