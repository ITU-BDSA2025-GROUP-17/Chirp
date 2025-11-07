namespace Chirp.Web.Pages;

using Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UserTimelineModel : PageModel
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }
    
    public string Text;

    public UserTimelineModel(ICheepRepository cheepRepository,IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public async Task<ActionResult> OnGet(string author)
    {
        string? page = HttpContext.Request.Query["page"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        Cheeps = await _cheepRepository.ReadCheeps(author, (pageNum - 1) * 32, 32);
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
        var author = await _authorRepository.GetAuthorByName(user);

        var cheep = new CheepDTO
        {
            Author = author,
            Text = Text,
            TimeStamp = DateTime.Now
        };
        await _cheepRepository.CreateCheep(cheep);
        Cheeps = await _cheepRepository.ReadCheeps(null, (pageNum - 1) * 32, 32);
        
        
        return RedirectToPage("UserTimeline");
    }
}
