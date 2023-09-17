namespace TobyMeehan.Com.Builders;

public struct StartSessionBuilder
{
    public StartSessionBuilder WithCanRefresh(bool value) => this with { CanRefresh = value };
    public bool CanRefresh { get; set; }
}