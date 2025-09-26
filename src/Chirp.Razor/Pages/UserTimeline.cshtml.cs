using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        string? page = HttpContext.Request.Query["page"];
        int page_num = 1;
        if (page != null)
        {
            page_num = int.Parse(page);
        }
        Cheeps = _service.GetCheepsFromAuthor(author, page_num);
        return Page();
    }
}
