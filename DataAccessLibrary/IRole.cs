namespace TobyMeehan.Com.Data
{
    public interface IRole
    {
        Id<IRole> Id { get; }
        
        string Name { get; }
    }
}