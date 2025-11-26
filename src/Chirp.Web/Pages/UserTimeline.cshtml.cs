namespace Chirp.Web.Pages;

using Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UserTimelineModel : PageModel
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }
    
    [BindProperty]
    public string? Text  { get; set; }
    
    [BindProperty]
    public string? Follow { get; set; }

    [BindProperty]
    public string? Unfollow { get; set; }

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

        Cheeps = await _cheepRepository.ReadCheepsFromFollowers(author, (pageNum - 1) * 32, 32);
        
        return Page();
    }
    
    public async Task<ActionResult> OnPostCheepAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

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
        
        return RedirectToPage("UserTimeline");
    }
    
    public async Task<ActionResult> OnPostUnfollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
   
        var user = User.Identity?.Name;
        var author = await _authorRepository.GetAuthorByName(user!);
        var followAuthor = await _authorRepository.GetAuthorByName(Unfollow!);
        await _authorRepository.UnFollow(author!, followAuthor!);


        return RedirectToPage("UserTimeline");
    }
    
    public async Task<bool> IsFollowingAsync(string currentUserName, string targetUserName)
    {
        // get curr DTO
        var currentUser = await _authorRepository.GetAuthorByName(currentUserName);

        // get auth DTO
        var targetUser = await _authorRepository.GetAuthorByName(targetUserName);
        if (targetUser == currentUser) throw new Exception("You cannot follow this yourself!");
        if (targetUser == null || currentUser == null) throw new Exception("null :(");
        var result = await _authorRepository.IsFollowing(currentUser, targetUser);

        return result;

    }
}
