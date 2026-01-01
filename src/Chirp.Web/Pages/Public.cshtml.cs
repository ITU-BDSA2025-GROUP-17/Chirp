namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Services;

/// <summary>
/// PageModel responsible for displaying the public timeline of cheeps.
/// </summary>
/// <remarks>
/// This page shows publicly available cheeps and supports pagination
/// and optional search filtering. It inherits common cheep-related
/// functionality from <see cref="CheepPageModel"/>.
/// </remarks>
public class PublicModel : CheepPageModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PublicModel"/> class.
    /// </summary>
    /// <param name="cheepService">
    /// Service responsible for retrieving cheeps.
    /// </param>
    /// <param name="authorService">
    /// Service responsible for author-related operations.
    /// </param>
    public PublicModel(ICheepService cheepService, IAuthorService authorService)
        : base(cheepService, authorService)
    {
    }

    /// <summary>
    /// Handles HTTP GET requests for the public timeline page.
    /// </summary>
    /// <remarks>
    /// This method retrieves public cheeps based on the current page index
    /// and optional search query provided via the request query string.
    /// </remarks>
    /// <returns>
    /// The public timeline page populated with cheeps.
    /// </returns>
    public async Task<ActionResult> OnGet()
    {
        // Retrieve the page index from the query string (defaults to 1)
        string? page = HttpContext.Request.Query["PageIndex"];
        int pageNum = 1;

        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        // Retrieve optional search query
        string? search = HttpContext.Request.Query["search"];

        // Load public cheeps for the given page and search term
        Cheeps = await _cheepService.GetPublicCheeps(pageNum, search);

        return Page();
    }
}