namespace Chirp.Web.Pages;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Repositories;
using Services;

/// <summary>
/// PageModel responsible for handling all interactions related to cheeps
/// on the main timeline page in the Chirp web application.
/// </summary>
/// <remarks>
/// This PageModel handles creating cheeps, following and unfollowing users,
/// saving and unsaving cheeps, searching, and preserving pagination state.
/// All business logic is delegated to the service layer.
/// </remarks>
public class CheepPageModel : PageModel
{
    /// <summary>
    /// Service responsible for cheep-related operations.
    /// </summary>
    protected readonly ICheepService _cheepService;

    /// <summary>
    /// Service responsible for author-related operations.
    /// </summary>
    protected readonly IAuthorService _authorService;

    /// <summary>
    /// The list of cheeps displayed on the page.
    /// </summary>
    public required List<CheepDTO> Cheeps { get; set; }

    /// <summary>
    /// Text content of a new cheep submitted by the user.
    /// </summary>
    [BindProperty]
    public string? Text { get; set; }

    /// <summary>
    /// Username of the author to follow.
    /// </summary>
    [BindProperty]
    public string? Follow { get; set; }

    /// <summary>
    /// Username of the author to unfollow.
    /// </summary>
    [BindProperty]
    public string? Unfollow { get; set; }

    /// <summary>
    /// ID of the cheep to save.
    /// </summary>
    [BindProperty]
    public long? Save { get; set; }

    /// <summary>
    /// ID of the cheep to remove from saved cheeps.
    /// </summary>
    [BindProperty]
    public long? Unsave { get; set; }

    /// <summary>
    /// Search term used to filter cheeps.
    /// Supports GET requests so it can be preserved across redirects.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public string? Search { get; set; }

    /// <summary>
    /// Current pagination index.
    /// Supports GET requests so it can be preserved across redirects.
    /// </summary>
    [BindProperty(SupportsGet = true)]
    public int? PageIndex { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CheepPageModel"/> class.
    /// </summary>
    /// <param name="cheepService">Service handling cheep operations.</param>
    /// <param name="authorService">Service handling author operations.</param>
    public CheepPageModel(ICheepService cheepService, IAuthorService authorService)
    {
        _cheepService = cheepService;
        _authorService = authorService;
    }

    /// <summary>
    /// Handles creation of a new cheep for the currently authenticated user.
    /// </summary>
    /// <returns>A redirect to the same page preserving search and pagination state.</returns>
    public async Task<ActionResult> OnPostCheepAsync()
    {
        if (Text == null || string.IsNullOrWhiteSpace(Text)|| !ModelState.IsValid)
        {
            ModelState.AddModelError("Text", "Your message is empty");
            return Page();
        } 
        
        string userName = User.Identity!.Name!;
        await _cheepService.CreateCheepForUser(userName, Text!);

        return RedirectToPage(null, new { search = Search, pageIndex = PageIndex });
    }

    /// <summary>
    /// Handles following another user.
    /// </summary>
    /// <returns>A redirect to the same page preserving search and pagination state.</returns>
    public async Task<ActionResult> OnPostFollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _authorService.FollowUser(userName, Follow!);

        return RedirectToPage(null, new { search = Search, pageIndex = PageIndex });
    }

    /// <summary>
    /// Handles unfollowing a previously followed user.
    /// </summary>
    /// <returns>A redirect to the same page preserving search and pagination state.</returns>
    public async Task<ActionResult> OnPostUnfollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _authorService.UnfollowUser(userName, Unfollow!);

        return RedirectToPage(null, new { search = Search, pageIndex = PageIndex });
    }

    /// <summary>
    /// Handles saving a cheep for the current user.
    /// </summary>
    /// <returns>A redirect to the same page preserving search and pagination state.</returns>
    public async Task<ActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _cheepService.SaveCheepForUser(userName, Save!.Value);

        return RedirectToPage(null, new { search = Search, pageIndex = PageIndex });
    }

    /// <summary>
    /// Handles removing a saved cheep for the current user.
    /// </summary>
    /// <returns>A redirect to the same page preserving search and pagination state.</returns>
    public async Task<ActionResult> OnPostRemoveSaveAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        string userName = User.Identity!.Name!;
        await _cheepService.RemoveSavedCheepForUser(userName, Unsave!.Value);

        return RedirectToPage(null, new { search = Search, pageIndex = PageIndex });
    }

    /// <summary>
    /// Determines whether the current user is following a given target user.
    /// </summary>
    /// <param name="currentUserName">Username of the current user.</param>
    /// <param name="targetUserName">Username of the target user.</param>
    /// <returns>
    /// True if the current user is following the target user; otherwise, false.
    /// </returns>
    public async Task<bool> IsFollowingAsync(string currentUserName, string targetUserName)
    {
        return await _authorService.IsFollowing(currentUserName, targetUserName);
    }

    /// <summary>
    /// Determines whether a given cheep is saved by the current user.
    /// </summary>
    /// <param name="cheep">The cheep to check.</param>
    /// <returns>
    /// True if the cheep is saved by the user; otherwise, false.
    /// </returns>
    public async Task<bool> IsSavedAsync(CheepDTO cheep)
    {
        string userName = User.Identity!.Name!;
        return await _cheepService.IsCheepSavedByUser(userName, cheep.CheepId!.Value);
    }

    /// <summary>
    /// Handles search form submission.
    /// Redirects to the same page with the search query applied.
    /// </summary>
    /// <returns>A redirect to the same page with the search parameter.</returns>
    public ActionResult OnPostSearch()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        return RedirectToPage(null, new { search = Search });
    }
}
