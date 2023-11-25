using Microsoft.Extensions.Options;
using TobyMeehan.Com.Data.Security.DataProtection;
using BC = BCrypt.Net.BCrypt;

namespace TobyMeehan.Com.Data.Security.Passwords;

public class BCryptPasswordService : IPasswordService
{
    private readonly IDataProtectionService _dataProtection;
    private readonly BCryptOptions _options;

    public BCryptPasswordService(IDataProtectionService dataProtection, IOptions<BCryptOptions> options)
    {
        _dataProtection = dataProtection;
        _options = options.Value;
    }
    
    public async Task<byte[]> HashAsync(Password password)
    {
        string hash = BC.HashPassword(password.ToString(), BC.GenerateSalt(_options.Rounds));

        byte[] bytes = await _dataProtection.ProtectAsync("TobyMeehan.Com.User.Password", hash);

        return bytes;
    }

    public async Task<PasswordHashResult> CheckAsync(Password password, byte[] protectedHash)
    {
        var hashResult = await _dataProtection.UnprotectAsync("TobyMeehan.Com.User.Password", protectedHash);

        if (hashResult.IsEmpty)
        {
            return new PasswordHashResult(false);
        }

        string hash = hashResult.Value;
        
        bool passwordIsCorrect = BC.Verify(password.ToString(), hash);

        if (passwordIsCorrect && BC.PasswordNeedsRehash(hash, _options.Rounds))
        {
            return new PasswordHashResult(passwordIsCorrect, await HashAsync(password));
        }

        return new PasswordHashResult(passwordIsCorrect);
    }
}