namespace TobyMeehan.Com;

/// <summary>
/// Represents the ID of an entity.
/// </summary>
/// <param name="Value"></param>
/// <typeparam name="T"></typeparam>
public record struct Id<T>(string Value);