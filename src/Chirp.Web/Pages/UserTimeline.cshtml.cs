namespace Chirp.Web.Pages;

using Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UserTimelineModel : CheepPageModel
{

    public UserTimelineModel(ICheepService cheepService, IAuthorService authorService)
        : base(cheepService, authorService)
    { }

    public async Task<ActionResult> OnGet(string author)
    {
        string? page = HttpContext.Request.Query["pageIndex"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        Cheeps = await _cheepService.GetUserTimelineCheeps(author, pageNum);

        return Page();
    }
}
