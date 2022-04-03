using System;

namespace TobyMeehan.Com.Data;

public class GuidIdGenerator : IIdGenerator
{
    public Id<T> GenerateId<T>()
    {
        return new Id<T>(Guid.NewGuid().ToString());
    }
}