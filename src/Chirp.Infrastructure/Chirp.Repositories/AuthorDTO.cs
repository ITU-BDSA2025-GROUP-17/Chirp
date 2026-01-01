namespace Chirp.Repositories;

/// <summary>
/// Data Transfer Object (DTO) representing an author.
/// Used to transfer author-related data between layers without exposing domain entities.
/// </summary>
public class AuthorDTO
{
    /// <summary>
    /// The display name of the author.
    /// </summary>
    public required string Name;
    /// <summary>
    /// The email address of the author.
    /// </summary>
    public required string Email;
    /// <summary>
    /// Unique identifier of the author.
    /// </summary>
    public int AuthorId;
    /// <summary>
    /// Cheeps written by the author.
    /// </summary>
    public ICollection<CheepDTO>? Messages;
    /// <summary>
    /// Other authors that this author is following.
    /// </summary>
    public ICollection<AuthorDTO>? Following;
}