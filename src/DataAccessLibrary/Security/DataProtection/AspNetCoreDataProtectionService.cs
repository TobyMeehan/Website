using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace TobyMeehan.Com.Data.Security.DataProtection;

public class AspNetCoreDataProtectionService : IDataProtectionService
{
    private readonly IDataProtectionProvider _provider;

    public AspNetCoreDataProtectionService(IDataProtectionProvider provider)
    {
        _provider = provider;
    }
    
    public Task<byte[]> ProtectAsync(string purpose, string data)
    {
        var protector = _provider.CreateProtector(purpose);

        byte[] protectedData = protector.Protect(Encoding.UTF8.GetBytes(data));
        
        return Task.FromResult(protectedData);
    }

    public Task<Optional<string>> UnprotectAsync(string purpose, byte[] protectedData)
    {
        var protector = _provider.CreateProtector(purpose);

        byte[]? unprotected = null;

        try
        {
            unprotected = protector.Unprotect(protectedData);
        }
        catch (CryptographicException)
        {
            
        }

        if (unprotected is null)
        {
            return Task.FromResult(Optional<string>.Empty());
        }

        var result = Optional<string>.Of(Encoding.UTF8.GetString(unprotected));

        return Task.FromResult(result);
    }
}