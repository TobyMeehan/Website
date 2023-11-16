namespace TobyMeehan.Com.Services;

public class QueryOptions
{
    public LimitStrategy LimitStrategy { get; set; } = new DefaultLimitStrategy();
}