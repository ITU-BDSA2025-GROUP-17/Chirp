using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

public class PublicModel : PageModel
{
    private readonly IAuthorRepository _AuthorRepository;
    private readonly ICheepRepository _CheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public string? Text  { get; set; }

    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _CheepRepository = cheepRepository;
        _AuthorRepository = authorRepository;
    }

    public async Task<ActionResult> OnGet()
    {
        string? page = HttpContext.Request.Query["page"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        Cheeps = await _CheepRepository.ReadCheeps(null, (pageNum-1)*32, 32);
        return Page();
    }
    
    public async Task<ActionResult> OnPostAsync(string Text)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        string? page = HttpContext.Request.Query["page"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }
        
        this.Text = Text;
        Console.WriteLine(Text);
        var user = User.Identity?.Name;
        Console.WriteLine(user);
        var author = await _AuthorRepository.GetAuthorByName(user);

        var cheep = new CheepDTO
        {
            Author = author,
            Text = Text,
            TimeStamp = DateTime.Now
        };
        await _CheepRepository.CreateCheep(cheep);
        Cheeps = await _CheepRepository.ReadCheeps(null, (pageNum - 1) * 32, 32);
        
        
        return RedirectToPage("Public");
    }
    
}
