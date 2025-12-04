namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repositories;

public class CheepPageModel : PageModel
{
    protected readonly IAuthorRepository _authorRepository;
    protected readonly ICheepRepository _cheepRepository;
    public required List<CheepDTO> Cheeps { get; set; }

    [BindProperty]
    public string? Text { get; set; }
    [BindProperty]
    public string? Follow { get; set; }
    [BindProperty]
    public string? Unfollow { get; set; }
    [BindProperty]
    public long? Save { get; set; }
    [BindProperty]
    public long? Unsave { get; set; }

    // For remembering search & Page when redirecting
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? PageIndex { get; set; }

    public CheepPageModel(ICheepRepository cheepRepository, IAuthorRepository authorRepository)
    {
        _cheepRepository = cheepRepository;
        _authorRepository = authorRepository;
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

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
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

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});

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

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
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

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
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

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
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

        return RedirectToPage(null, new { search = Search });
    }
}
