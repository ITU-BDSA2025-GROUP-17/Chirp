namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Repositories;
using Services;

public class CheepPageModel : PageModel
{
    protected readonly ICheepService _cheepService;
    protected readonly IAuthorService _authorService;
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

    public CheepPageModel(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }

    public async Task<ActionResult> OnPostCheepAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _cheepService.CreateCheepForUser(userName, Text!);

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
    }

    public async Task<ActionResult> OnPostFollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _authorService.FollowUser(userName, Follow!);

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
    }

    public async Task<ActionResult> OnPostUnfollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _authorService.UnfollowUser(userName, Unfollow!);

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
    }

    public async Task<ActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _cheepService.SaveCheepForUser(userName, Save!.Value);

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
    }

    public async Task<ActionResult> OnPostRemoveSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _cheepService.RemoveSavedCheepForUser(userName, Unsave!.Value);

        return RedirectToPage(null, new {search = Search, pageIndex = PageIndex});
    }

    public async Task<bool> IsFollowingAsync(string currentUserName, string targetUserName)
    {
        return await _authorService.IsFollowing(currentUserName, targetUserName);
    }

    public async Task<bool> IsSavedAsync(CheepDTO cheep)
    {
        string userName = User.Identity!.Name!;
        return await _cheepService.IsCheepSavedByUser(userName, cheep.CheepId!.Value);
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
