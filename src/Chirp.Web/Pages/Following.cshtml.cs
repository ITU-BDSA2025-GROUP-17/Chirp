using Chirp.Core;
using Chirp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

/// <summary>
/// PageModel responsible for displaying and managing the list of authors
/// that the currently authenticated user is following.
/// </summary>
/// <remarks>
/// This page allows the user to view all followed authors and unfollow them.
/// Data access is performed through the <see cref="IAuthorRepository"/>.
/// </remarks>
public class FollowingModel : PageModel
{
    /// <summary>
    /// Repository used to retrieve and modify author follow relationships.
    /// </summary>
    private readonly IAuthorRepository _authorRepository;

    /// <summary>
    /// Collection of authors that the current user is following.
    /// </summary>
    public ICollection<AuthorDTO>? Following { get; set; }

    /// <summary>
    /// Username of the author to unfollow.
    /// Bound from the unfollow form submission.
    /// </summary>
    [BindProperty]
    public string? Unfollow { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FollowingModel"/> class.
    /// </summary>
    /// <param name="authorRepository">
    /// Repository responsible for author persistence and relationships.
    /// </param>
    public FollowingModel(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Handles HTTP GET requests for the Following page.
    /// Retrieves all authors that the current user is following.
    /// </summary>
    /// <returns>
    /// The Following page if the user is authenticated; otherwise,
    /// a redirect to the Index page.
    /// </returns>
    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }

        var authorName = User.Identity.Name;

        // Retrieve the current user as an author entity
        var currentUser = await _authorRepository.GetAuthorByName(authorName!);

        if (currentUser != null)
        {
            // Load the list of followed authors
            Following = await _authorRepository.GetFollowing(currentUser);
        }

        return Page();
    }

    /// <summary>
    /// Handles unfollow requests submitted from the Following page.
    /// </summary>
    /// <returns>
    /// A redirect to the Following page after the unfollow operation completes.
    /// </returns>
    public async Task<ActionResult> OnPostUnfollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userName = User.Identity!.Name;

        // Retrieve both the current user and the author to unfollow
        var currentAuthor = await _authorRepository.GetAuthorByName(userName!);
        var followAuthor = await _authorRepository.GetAuthorByName(Unfollow!);

        // Remove the follow relationship
        await _authorRepository.UnFollow(currentAuthor!, followAuthor!);

        return RedirectToPage("/Following");
    }
}
