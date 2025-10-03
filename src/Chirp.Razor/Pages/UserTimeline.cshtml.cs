using Chirp.Razor.wwwroot;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepRepository _repository;
    public List<CheepDTO> Cheeps { get; set; }

    public UserTimelineModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResult> OnGet(string author)
    {
        /*string? page = HttpContext.Request.Query["page"];
        int page_num = 1;
        if (page != null)
        {
            page_num = int.Parse(page);
        }*/
        Cheeps = await _repository.ReadCheeps(author);
        return Page();
    }
}
