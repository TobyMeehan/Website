using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TobyMeehan.Com.Data;

public interface IScoreboardRepository
{
    Task<IObjective> GetByIdAsync(Id<IObjective> id);

    Task<IReadOnlyList<IObjective>> GetByApplicationAsync(Id<IApplication> appId);
    
    Task<IObjective> AddObjectiveAsync(Action<NewObjective> objective);

    Task SetScoreAsync(Id<IObjective> objectiveId, Id<IUser> userId, int value);

    Task DeleteAsync(Id<IObjective> id);
}

public class NewObjective
{
    public Id<IApplication>? AppId { get; set; } = null;
    public string Name { get; set; } = null;
}