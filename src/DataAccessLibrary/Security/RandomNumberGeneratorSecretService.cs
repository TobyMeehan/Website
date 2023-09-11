using System.Security.Cryptography;
using System.Text;

namespace TobyMeehan.Com.Data.Security;

public class RandomNumberGeneratorSecretService : ISecretService
{
    private static readonly char[] _chars =
        "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890".ToCharArray();
    
    public Task<string> GenerateSecretAsync(int length)
    {
        byte[] buffer = new byte[4 * length];

        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(buffer);
        }

        var result = new StringBuilder(length);

        for (int i = 0; i < length; i++)
        {
            uint position = BitConverter.ToUInt32(buffer, i * 4);
            long index = position % _chars.Length;

            result.Append(_chars[index]);
        }

        return Task.FromResult(result.ToString());
    }
}