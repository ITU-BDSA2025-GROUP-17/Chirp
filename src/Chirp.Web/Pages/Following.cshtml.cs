
using Chirp.Repositories;
using Chirp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class FollowingModel : PageModel
{
    /// <summary>
    /// Service used to retrieve and modify author follow relationships.
    /// </summary>
    private readonly IAuthorService _authorService;

    public ICollection<AuthorDTO>? Following { get; set; }

    [BindProperty]
    public string? Unfollow { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="FollowingModel"/> class.
    /// </summary>
    /// <param name="authorService">
    /// Repository responsible for author persistence and relationships.
    /// </param>
    public FollowingModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }

        var authorName = User.Identity.Name;

        // Retrieve the current user as an author entity
        var currentUser = await _authorService.GetAuthorByName(authorName!);



        if (currentUser != null)
        {
            // Load the list of followed authors
            Following = await _authorService.GetFollowing(currentUser.Name);
        }

        return Page();
    }

    public async Task<ActionResult> OnPostUnfollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = User.Identity?.Name;
        var author = await _authorService.GetAuthorByName(user!);
        var followAuthor = await _authorService.GetAuthorByName(Unfollow!);
        await _authorService.UnfollowUser(author!.Name!, followAuthor!.Name!);

        return RedirectToPage("/Following");
    }

}
