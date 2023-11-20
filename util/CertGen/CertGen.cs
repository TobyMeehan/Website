using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertGen;

public class CertGen
{
    public static X509Certificate2 GenerateCertificate(string name, DateTimeOffset notBefore, DateTimeOffset notAfter, X509KeyUsageFlags keyUsages)
    {
        using var algorithm = RSA.Create(2048);

        var subject = new X500DistinguishedName($"CN={name}");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(keyUsages, critical: true));

        var certificate = request.CreateSelfSigned(notBefore, notAfter);

        return certificate;
    }
}