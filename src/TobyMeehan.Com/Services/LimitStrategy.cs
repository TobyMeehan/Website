namespace TobyMeehan.Com.Services;

public abstract class LimitStrategy
{
    public abstract int GetOffset();
    
    public abstract int GetLimit();
}

public class OffsetLimitStrategy : LimitStrategy
{
    public int Limit { get; }
    public int Offset { get; }

    public OffsetLimitStrategy(int limit, int offset)
    {
        Limit = limit;
        Offset = offset;
    }

    public override int GetOffset() => Offset;

    public override int GetLimit() => Limit;
}

public class PageLimitStrategy : LimitStrategy
{
    public int Page { get; }
    public int PerPage { get; }

    public PageLimitStrategy(int page, int perPage)
    {
        Page = page;
        PerPage = perPage;
    }

    public override int GetOffset()
    {
        return (Page - 1) * PerPage;
    }
    
    public override int GetLimit()
    {
        return PerPage;
    }
}

public class DefaultLimitStrategy : LimitStrategy
{
    public override int GetOffset() => 0;

    public override int GetLimit() => 200;
}