namespace Chirp.Repositories;

/// <summary>
/// Data Transfer Object (DTO) representing a cheep.
/// Used to transfer cheep data between layers without exposing domain entities.
/// </summary>
public class CheepDTO
{
    /// <summary>
    /// The author who wrote the cheep.
    /// </summary>
    public required AuthorDTO Author;

    /// <summary>
    /// The textual content of the cheep.
    /// </summary>
    public required string Text;

    /// <summary>
    /// The time when the cheep was created.
    /// </summary>
    public required DateTime TimeStamp;

    /// <summary>
    /// Optional unique identifier of the cheep.
    /// </summary>
    public long? CheepId;
}