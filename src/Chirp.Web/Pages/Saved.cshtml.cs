namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Services;
using Repositories;

public class SavedModel : CheepPageModel
{

    public SavedModel(ICheepService cheepService, IAuthorService authorService)
        : base(cheepService, authorService)
    { }

    public async Task<ActionResult> OnGet()
    {
        string? page = HttpContext.Request.Query["pageIndex"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        if(User.Identity != null) {
            Cheeps = await _cheepService.GetSavedCheeps(User.Identity!.Name, pageNum);
        } else
        {
            Cheeps = new List<CheepDTO>();
        }
        return Page();
    }
}
