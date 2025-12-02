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
    public string? Text { get; set; }
    [BindProperty]
    public string? SearchText { get; set; }
    [BindProperty]
    public string? Follow { get; set; }
    [BindProperty]
    public string? Unfollow { get; set; }
    [BindProperty]
    public long? Save { get; set; }
    [BindProperty]
    public long? Unsave { get; set; }

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

        string? search = HttpContext.Request.Query["search"];
        if (search != null)
        {
            Cheeps = await _cheepRepository.ReadCheepsWithSearch(null, search, (pageNum - 1) * 32, 32);
        } else
        {
            Cheeps = await _cheepRepository.ReadCheeps(null, (pageNum - 1) * 32, 32);
        }
        return Page();
    }

    public async Task<ActionResult> OnPostCheepAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = User.Identity?.Name;
        var author = await _authorRepository.GetAuthorByName(user!);

        var cheep = new CheepDTO
        {
            Author = author!,
            Text = Text!,
            TimeStamp = DateTime.Now
        };
        await _cheepRepository.CreateCheep(cheep);

        return RedirectToPage("Public");
    }

    public async Task<ActionResult> OnPostFollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var user = User.Identity?.Name;
        var author = await _authorRepository.GetAuthorByName(user!);


        var followAuthor = await _authorRepository.GetAuthorByName(Follow!);
        await _authorRepository.Follow(author!, followAuthor!);


        return RedirectToPage("Public");

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


        return RedirectToPage("Public");
    }

    public async Task<ActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var author = await _authorRepository.GetAuthorByName(User.Identity!.Name!);
        var cheep = await _cheepRepository.GetCheepById((long)Save!);

        await _cheepRepository.SaveCheep(author!, cheep!);

        return RedirectToPage("Public");
    }

    public async Task<ActionResult> OnPostRemoveSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        var author = await _authorRepository.GetAuthorByName(User.Identity!.Name!);
        var cheep = await _cheepRepository.GetCheepById((long)Unsave!);

        await _cheepRepository.RemoveSavedCheep(author!, cheep!);

        return RedirectToPage("Public");
    }

    // Curr follows target....
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

    public async Task<bool> IsSavedAsync(CheepDTO cheep)
    {
        var author = await _authorRepository.GetAuthorByName(User.Identity!.Name!);
        
        return await _cheepRepository.IsSaved(author!, cheep);

    }

    public ActionResult OnPostSearch()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        return RedirectToPage("Public", new { search = SearchText });
    }
}
