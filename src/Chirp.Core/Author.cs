namespace Chirp.Core;

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class Author : IdentityUser<int>
{
    public long AuthorId { get; set; }
    public ICollection<Cheep>? Cheeps { get; set; }
}