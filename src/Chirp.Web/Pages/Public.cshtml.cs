using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

public class PublicModel : PageModel
{
    private readonly ICheepRepository _repository;
    public required List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public string? Text  { get; set; }

    public PublicModel(ICheepRepository repository)
    {
        _repository = repository;
    }

    public async Task<ActionResult> OnGet()
    {
        string? page = HttpContext.Request.Query["page"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        Cheeps = await _repository.ReadCheeps(null, (pageNum-1)*32, 32);
        return Page();
    }
    
    public ActionResult Post(string Text)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        this.Text = Text;
        Console.WriteLine(Text);
        return RedirectToPage("Public");
    }
}
