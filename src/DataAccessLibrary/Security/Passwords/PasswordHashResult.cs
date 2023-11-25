namespace TobyMeehan.Com.Data.Security.Passwords;

public class PasswordHashResult
{
    public PasswordHashResult(bool succeeded)
    {
        Succeeded = succeeded;
    }
    
    public PasswordHashResult(bool succeeded, Optional<byte[]> rehash)
    {
        Succeeded = succeeded;
        Rehash = rehash;
    }

    public bool Succeeded { get; }
    public Optional<byte[]> Rehash { get; }
}