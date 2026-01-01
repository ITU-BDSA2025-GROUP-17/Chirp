namespace Chirp.Web.Pages;

using Services;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// PageModel responsible for displaying the timeline of cheeps
/// for a specific user.
/// </summary>
/// <remarks>
/// This page shows all cheeps authored by a given user and supports
/// pagination. It inherits shared cheep-related functionality from
/// <see cref="CheepPageModel"/>.
/// </remarks>
public class UserTimelineModel : CheepPageModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserTimelineModel"/> class.
    /// </summary>
    /// <param name="cheepService">
    /// Service responsible for retrieving cheeps.
    /// </param>
    /// <param name="authorService">
    /// Service responsible for author-related operations.
    /// </param>
    public UserTimelineModel(ICheepService cheepService, IAuthorService authorService)
        : base(cheepService, authorService)
    {
    }

    /// <summary>
    /// Handles HTTP GET requests for a user's timeline page.
    /// </summary>
    /// <remarks>
    /// This method retrieves cheeps authored by the specified user,
    /// using the provided author parameter and the current page index
    /// for pagination.
    /// </remarks>
    /// <param name="author">
    /// Username of the author whose timeline is being displayed.
    /// </param>
    /// <returns>
    /// The user's timeline page populated with cheeps.
    /// </returns>
    public async Task<ActionResult> OnGet(string author)
    {
        // Retrieve the page index from the query string (defaults to 1)
        string? page = HttpContext.Request.Query["pageIndex"];
        int pageNum = 1;

        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        // Load cheeps for the specified user's timeline
        var username = User.Identity!.Name;
        Cheeps = await _cheepService.GetUserTimelineCheeps(username!, author, pageNum);

        return Page();
    }
}
