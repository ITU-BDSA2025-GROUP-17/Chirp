
using Chirp.Repositories;
using Chirp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class FollowingModel : PageModel
{
    private readonly IAuthorService _authorService;

    public ICollection<AuthorDTO>? Following { get; set; }

    [BindProperty]
    public string? Unfollow { get; set; }

    public FollowingModel(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }

        var authorName = User.Identity.Name;


        var currentUser = await _authorService.GetAuthorByName(authorName!);



        if (currentUser != null)
        {
            Following = await _authorService.GetFollowing(currentUser.Name);
        }

        return Page();
    }

    public async Task<ActionResult> OnPostUnfollowAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = User.Identity?.Name;
        var author = await _authorService.GetAuthorByName(user!);
        var followAuthor = await _authorService.GetAuthorByName(Unfollow!);
        await _authorService.UnfollowUser(author!.Name!, followAuthor!.Name!);


        return RedirectToPage("/Following");
    }

}
