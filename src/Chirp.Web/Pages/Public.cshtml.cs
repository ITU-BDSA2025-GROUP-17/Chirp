namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Services;

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
    { }

    public async Task<ActionResult> OnGet()
    {
        string? page = HttpContext.Request.Query["PageIndex"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        string? search = HttpContext.Request.Query["search"];
        Cheeps = await _cheepService.GetPublicCheeps(pageNum, search);

        return Page();
    }
}
