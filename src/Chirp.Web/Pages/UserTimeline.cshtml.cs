namespace Chirp.Web.Pages;

using Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UserTimelineModel : CheepPageModel
{

    public UserTimelineModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository) 
        : base(cheepRepository, authorRepository)
    { }

    public async Task<ActionResult> OnGet(string author)
    {
        string? page = HttpContext.Request.Query["pageIndex"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        Cheeps = await _cheepRepository.ReadCheepsFromFollowers(author, (pageNum - 1) * 32, 32);
        
        return Page();
    }
}
