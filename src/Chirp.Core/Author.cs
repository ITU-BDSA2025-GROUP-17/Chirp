namespace Chirp.Core;

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

/// <summary>
/// Represents a user (author) in the Chirp system.
/// An author can create cheeps, save cheeps, and follow other authors.
/// </summary>
public class Author : IdentityUser<int>
{
    /// <summary>
    /// Cheeps written by this author.
    /// </summary>
    public ICollection<Cheep>? Cheeps { get; set; }
    /// <summary>
    /// Cheeps saved/bookmarked by this author.
    /// </summary>
    public ICollection<SavedCheep>? SavedCheeps { get; set; }
    /// <summary>
    /// Other authors that this author is following.
    /// </summary>
    public ICollection<Follow>? Following { get; set; }

}