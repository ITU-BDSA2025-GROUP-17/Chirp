using Chirp.Core;
using Chirp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class FollowingModel : PageModel
{
    private readonly IAuthorRepository _authorRepository;

    public ICollection<AuthorDTO>? Following { get; set; }

    public FollowingModel(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    public async Task<IActionResult> OnGetAsync()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return RedirectToPage("/Index");
        }

        var authorName = User.Identity.Name;


        var currentUser = await _authorRepository.GetAuthorByName(authorName!);


        if (currentUser != null)
        {
            Following = await _authorRepository.GetFollowing(currentUser);
        }

        return Page();
    }

}
