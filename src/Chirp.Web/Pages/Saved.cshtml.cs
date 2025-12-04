namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

public class SavedModel : CheepPageModel
{

    public SavedModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository) 
        : base(cheepRepository, authorRepository)
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
            Cheeps = await _cheepRepository.ReadSavedCheeps(User.Identity!.Name, (pageNum - 1) * 32, 32);
        } else
        {
            Cheeps = new List<CheepDTO>();
        }
        return Page();
    }
}
