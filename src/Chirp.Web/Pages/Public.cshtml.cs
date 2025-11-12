namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

public class PublicModel : PageModel
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public string? Text  { get; set; }
    
    public PublicModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
    }

    public async Task<ActionResult> OnGet()
    {
        string? page = HttpContext.Request.Query["page"];
        int pageNum = 1;
        if (page != null)
        {
            pageNum = int.Parse(page);
        }

        Cheeps = await _cheepRepository.ReadCheeps(null, (pageNum-1)*32, 32);
        return Page();
    }
    
    public async Task<ActionResult> OnPostAsync()
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
        var author = await _authorRepository.GetAuthorByName(user!);

        var cheep = new CheepDTO
        {
            Author = author!,
            Text = Text!,
            TimeStamp = DateTime.Now
        };
        await _cheepRepository.CreateCheep(cheep);
        Cheeps = await _cheepRepository.ReadCheeps(null, (pageNum - 1) * 32, 32);
        
        
        return RedirectToPage("Public");
    }

    // Curr follows target....
    public async Task<bool> IsFollowingAsync(string currentUserName, string targetUserName)
    {
        // get curr DTO
        var currentUser = await _authorRepository.GetAuthorByName(currentUserName);
        
        // get auth DTO
        var  targetUser = await _authorRepository.GetAuthorByName(targetUserName);
        if(targetUser == currentUser) throw new Exception("You cannot follow this yourself!");
        if (targetUser == null || currentUser == null) throw new Exception("null :("); 
        var result = await _authorRepository.IsFollowing(currentUser, targetUser);
        
        return result;

    }
    
}
