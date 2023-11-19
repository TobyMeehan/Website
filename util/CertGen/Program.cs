// Generates a signing and encryption certificate to be used for OpenID Connect tokens.

// Production certs should be generated locally and stored securely (not added to source control lol).

using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

Console.Write("File path for encryption certificate: ");
string? encryptionCertificatePath = Console.ReadLine();

Console.WriteLine();

Console.Write("Name for encryption certificate: ");
string? encryptionCertificateName = Console.ReadLine();

Console.WriteLine();

using (var algorithm = RSA.Create(2048))
{
    var subject = new X500DistinguishedName($"CN={encryptionCertificateName}");
    var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

    request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));

    var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

    await File.WriteAllBytesAsync(encryptionCertificatePath!, certificate.Export(X509ContentType.Pfx, string.Empty));
}

Console.WriteLine($"Generated encryption certificate {encryptionCertificateName} at '{encryptionCertificatePath}'");

Console.Write("File path for signing certificate: ");
string? signingCertificatePath = Console.ReadLine();

Console.WriteLine();

Console.Write("Name for signing certificate: ");
string? signingCertificateName = Console.ReadLine();

Console.WriteLine();

using (var algorithm = RSA.Create(2048))
{
    var subject = new X500DistinguishedName($"CN={signingCertificateName}");
    var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

    var certificate = request.CreateSelfSigned(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2));

    await File.WriteAllBytesAsync(signingCertificatePath!, certificate.Export(X509ContentType.Pfx, string.Empty));
}

Console.WriteLine($"Generated signing certificate {signingCertificateName} at '{signingCertificatePath}'");