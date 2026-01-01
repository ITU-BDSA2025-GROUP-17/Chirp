namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Repositories;

/// <summary>
/// PageModel responsible for displaying the list of cheeps saved by
/// the currently authenticated user.
/// </summary>
/// <remarks>
/// This page shows cheeps that a user has explicitly saved.
/// It supports pagination and inherits shared cheep functionality
/// from <see cref="CheepPageModel"/>.
/// </remarks>
public class SavedModel : CheepPageModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SavedModel"/> class.
    /// </summary>
    /// <param name="cheepService">
    /// Service responsible for cheep-related operations.
    /// </param>
    /// <param name="authorService">
    /// Service responsible for author-related operations.
    /// </param>
    public SavedModel(ICheepService cheepService, IAuthorService authorService)
        : base(cheepService, authorService)
    {
    }

    /// <summary>
    /// Handles HTTP GET requests for the Saved cheeps page.
    /// </summary>
    /// <remarks>
    /// This method retrieves cheeps saved by the current user based on
    /// the requested page index. If the user is not authenticated,
    /// an empty list is returned.
    /// </remarks>
    /// <returns>
    /// The Saved cheeps page populated with the user's saved cheeps.
    /// </returns>
    public async Task<ActionResult> OnGet()
    {
        // Retrieve the page index from the query string (defaults to 1)
        string? page = HttpContext.Request.Query["pageIndex"];
        int pageNum = 1;

        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        // Load saved cheeps for the authenticated user, if available
        if (User.Identity != null)
        {
            Cheeps = await _cheepService.GetSavedCheeps(User.Identity!.Name!, pageNum);
        }
        else
        {
            Cheeps = new List<CheepDTO>();
        }

        return Page();
    }
}