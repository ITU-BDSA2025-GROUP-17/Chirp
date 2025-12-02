namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;

public class SavedModel : PageModel
{
    private readonly IAuthorRepository _authorRepository;
    private readonly ICheepRepository _cheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public string? Follow { get; set; }

    [BindProperty]
    public string? Unfollow { get; set; }
    [BindProperty]
    public long? Unsave { get; set; }

    public SavedModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
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
        
        if(User.Identity != null) {
            Cheeps = await _cheepRepository.ReadSavedCheeps(User.Identity!.Name, (pageNum - 1) * 32, 32);
        } else
        {
            Cheeps = new List<CheepDTO>();
        }
        return Page();
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


        return RedirectToPage("Saved");

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


        return RedirectToPage("Saved");
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

        return RedirectToPage("Saved");
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
}
