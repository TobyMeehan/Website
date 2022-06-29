namespace TobyMeehan.Com;

public struct MediaType
{
    public MediaType(string type, string subType)
    {
        Type = type;
        SubType = subType;
    }

    public string Type { get; }
    public string SubType { get; }

    public override string ToString()
    {
        return $"{Type}/{SubType}";
    }
}