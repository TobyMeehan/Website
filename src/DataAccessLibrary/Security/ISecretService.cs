namespace TobyMeehan.Com.Data.Security;

/// <summary>
/// Service providing cryptographically secure random strings for use as secrets and keys. 
/// </summary>
public interface ISecretService
{
    Task<string> GenerateSecretAsync(int length);
}