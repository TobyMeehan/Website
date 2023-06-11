namespace TobyMeehan.Com.Data.Entities;

public class User : Entity<IUser>, IUser
{
    public User(string id, string name, string handle, double balance, string? description = null, IFile? avatar = null) : base(id)
    {
        Name = name;
        Handle = handle;
        Balance = balance;
        Description = description;
        Avatar = avatar;
    }
    
    public string Name { get; }
    public string Handle { get; }
    public double Balance { get; }
    public string? Description { get; }
    public IFile? Avatar { get; }
}