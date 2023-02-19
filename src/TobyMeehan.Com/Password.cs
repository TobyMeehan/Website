namespace TobyMeehan.Com;

public class Password
{
    private readonly string _value;

    public Password(string password)
    {
        _value = password;
    }

    public static implicit operator Password(string value)
    {
        return new Password(value);
    }

    public override string ToString()
    {
        return _value;
    }
}