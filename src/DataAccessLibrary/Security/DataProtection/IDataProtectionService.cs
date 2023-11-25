namespace TobyMeehan.Com.Data.Security.DataProtection;

public interface IDataProtectionService
{
    Task<byte[]> ProtectAsync(string purpose, string data);

    Task<Optional<string>> UnprotectAsync(string purpose, byte[] protectedData);
}