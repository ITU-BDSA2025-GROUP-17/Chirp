namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repositories;

public class PublicModel : CheepPageModel
{

    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository) 
        : base(cheepRepository, authorRepository)
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
        if (search != null)
        {
            Cheeps = await _cheepRepository.ReadCheepsWithSearch(null, search, (pageNum - 1) * 32, 32);
        } else
        {
            Cheeps = await _cheepRepository.ReadCheeps(null, (pageNum - 1) * 32, 32);
        }
        return Page();
    }
}
