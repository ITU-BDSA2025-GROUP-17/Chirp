namespace Chirp.Core;

using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class Author : IdentityUser<int>
{
    public ICollection<Cheep>? Cheeps { get; set; }
    public ICollection<Follow>? Following { get; set; }

}