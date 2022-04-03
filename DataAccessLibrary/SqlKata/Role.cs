namespace TobyMeehan.Com.Data.SqlKata;

public class Role : IRole
{
    public Id<IRole> Id { get; }
    public string Name { get; }
}