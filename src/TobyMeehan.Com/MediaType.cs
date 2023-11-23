using MimeTypes;

namespace TobyMeehan.Com;

public struct MediaType
{
    public MediaType(string type, string subType, string extension)
    {
        Type = type;
        SubType = subType;
        Extension = extension;
    }

    public string Type { get; }
    public string SubType { get; }
    public string Extension { get; }

    public override string ToString()
    {
        return $"{Type}/{SubType}";
    }

    public static MediaType Parse(string value)
    {
        string[] split = value.Split('/');

        return new MediaType(split[0], split[1], MimeTypeMap.GetExtension(value));
    }
}