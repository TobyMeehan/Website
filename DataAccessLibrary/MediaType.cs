using System;

namespace TobyMeehan.Com.Data;

public class MediaType
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

    public static MediaType Parse(string value)
    {
        var split = value.Split('/');

        if (split.Length != 2)
        {
            throw new ArgumentException("Media type should have only type and subtype.", nameof(value));
        }

        return new MediaType(split[0], split[1]);
    }

    public static bool TryParse(string value, out MediaType mediaType)
    {
        try
        {
            mediaType = Parse(value);
            return true;
        }
        catch (Exception e)
        {
            mediaType = null;
            return false;
        }
    }
}