using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Razor.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepViewModel> Cheeps { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        string? page = HttpContext.Request.Query["page"];
        int page_num = 1;
        if (page != null)
        {
            page_num = int.Parse(page);
        }
        Cheeps = _service.GetCheeps(page_num);
        return Page();
    }
}
