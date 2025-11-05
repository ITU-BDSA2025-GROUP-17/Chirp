namespace Chirp.Web.Pages;

using Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    public required List<CheepDTO> Cheeps { get; set; }
    
    public string Text;

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResult> OnGet(string author)
    {
        string? page = HttpContext.Request.Query["page"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        Cheeps = await _repository.ReadCheeps(author, (pageNum - 1) * 32, 32);
        return Page();
    }
}
