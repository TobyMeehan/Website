using TobyMeehan.Com.Data.Configuration;
using TobyMeehan.Com.Data.Security.DataProtection;
using TobyMeehan.Com.Data.Security.Passwords;

namespace TobyMeehan.Com.Data.Security.Configuration;

public static class DataAccessLibraryExtensions
{
    public static DataAccessLibraryBuilder AddBCryptPasswordHash(this DataAccessLibraryBuilder builder,
        Action<BCryptOptions> configureOptions)
    {
        var options = new BCryptOptions();

        configureOptions(options);

        return AddBCryptPasswordHash(builder, options);
    }
    
    internal static DataAccessLibraryBuilder AddBCryptPasswordHash(this DataAccessLibraryBuilder builder,
        BCryptOptions options)
    {
        return builder.AddPasswordHasher<BCryptPasswordService>();
    }

    public static DataAccessLibraryBuilder AddAspNetCoreDataProtection(this DataAccessLibraryBuilder builder)
    {
        return builder.AddDataProtection<AspNetCoreDataProtectionService>();
    }
}