namespace TobyMeehan.Com.Data;

public interface IScore
{
    Id<IScore> Id { get; }
    Id<IObjective> ObjectiveId { get; }
    Id<IUser> UserId { get; }
    
    IUser User { get; }
    
    int Value { get; }
}