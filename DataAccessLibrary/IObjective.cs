using System.Collections.Generic;

namespace TobyMeehan.Com.Data;

public interface IObjective
{
    Id<IObjective> Id { get; }
    Id<IApplication> AppId { get; }
    
    string Name { get; }
    
    IReadOnlyList<IScore> Scores { get; }
}