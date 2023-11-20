using System.Security.Cryptography.X509Certificates;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace CertGen;

[Command("sign", Description = "Generates an X509 signing certificate.")]
public class SignCommand : ICommand
{
    [CommandParameter(0, Name = "Name of the certificate.")]
    public string? Name { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        string name = Name ?? "Signing Certificate";
        
        var certificate = CertGen.GenerateCertificate(name, 
            DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(2), 
            X509KeyUsageFlags.DigitalSignature);

        byte[] bytes = certificate.Export(X509ContentType.Pfx);
        
        string base64 = Convert.ToBase64String(bytes);
        
        console.Output.WriteLine();
        
        console.Output.WriteLine($"Signing Certificate '{name}':");
        
        console.Output.WriteLine();
        console.Output.WriteLine(base64);
        console.Output.WriteLine();
        
        console.Output.WriteLine($"Thumbprint: {certificate.Thumbprint}");
        
        console.Output.WriteLine();
        
        return default;
    }
}