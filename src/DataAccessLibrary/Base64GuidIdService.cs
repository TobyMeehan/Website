namespace TobyMeehan.Com.Data;

public class Base64GuidIdService : IIdService
{
    public Task<Id<T>> GenerateAsync<T>()
    {
        byte[] guid = Guid.NewGuid().ToByteArray();
        
        string id = Convert.ToBase64String(guid)
            .Replace("/", "-")
            .Replace("+", "_")
            .TrimEnd('=');
        
        return Task.FromResult(new Id<T>(id));
    }
}