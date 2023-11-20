using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace CertGen;

[Command("encrypt", Description = "Generates an X509 encryption certificate.")]
public class EncryptCommand : ICommand
{
    [CommandParameter(0, Description = "Name of the certificate.")]
    public string? Name { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        string name = Name ?? "Encryption Certificate";
        
        var certificate = CertGen.GenerateCertificate(name, 
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2), 
            X509KeyUsageFlags.KeyEncipherment);

        byte[] bytes = certificate.Export(X509ContentType.Pfx);
        
        string base64 = Convert.ToBase64String(bytes);

        console.Output.WriteLine();
        
        console.Output.WriteLine($"Encryption Certificate '{name}':");
        
        console.Output.WriteLine();
        console.Output.WriteLine(base64);
        console.Output.WriteLine();
        
        console.Output.WriteLine($"Thumbprint: {certificate.Thumbprint}");
        
        console.Output.WriteLine();
        
        return default;
    }

    
}