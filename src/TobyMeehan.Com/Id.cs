namespace TobyMeehan.Com;

/// <summary>
/// Represents the ID of an entity.
/// </summary>
/// <param name="Value">Plaintext form of the ID.</param>
/// <typeparam name="T">Type of entity to which the ID belongs.</typeparam>
public record struct Id<T>(string Value);