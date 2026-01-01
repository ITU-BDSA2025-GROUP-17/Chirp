namespace Chirp.Services;

using Repositories;

/// <summary>
/// Service responsible for cheep-related business logic.
/// </summary>
/// <remarks>
/// This service coordinates cheep operations between the web layer and the
/// <see cref="ICheepRepository"/> while enforcing application rules such as
/// pagination, validation, and user existence checks.
/// </remarks>
public class CheepService : ICheepService
{
    /// <summary>
    /// Repository used for cheep persistence and retrieval.
    /// </summary>
    private readonly ICheepRepository _cheepRepository;

    /// <summary>
    /// Service used to retrieve author information and validate user existence.
    /// </summary>
    private readonly IAuthorService _authorService;

    /// <summary>
    /// Number of cheeps displayed per page.
    /// </summary>
    private const int CheepsPerPage = 32;

    /// <summary>
    /// Initializes a new instance of the <see cref="CheepService"/> class.
    /// </summary>
    /// <param name="cheepRepository">
    /// Repository responsible for cheep data access.
    /// </param>
    /// <param name="authorService">
    /// Service responsible for author-related operations.
    /// </param>
    public CheepService(ICheepRepository cheepRepository, IAuthorService authorService)
    {
        _cheepRepository = cheepRepository;
        _authorService = authorService;
    }

    /// <summary>
    /// Retrieves publicly visible cheeps with optional search filtering.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="searchQuery">Optional search query for filtering cheeps.</param>
    /// <returns>
    /// A list of public cheeps for the specified page.
    /// </returns>
    public async Task<List<CheepDTO>> GetPublicCheeps(int pageNumber, string? searchQuery)
    {
        int offset = (pageNumber - 1) * CheepsPerPage;
        List<CheepDTO> cheeps;

        if (searchQuery != null)
        {
            cheeps = await _cheepRepository.ReadCheepsWithSearch(null, searchQuery, offset, CheepsPerPage);
        }
        else
        {
            cheeps = await _cheepRepository.ReadCheeps(null, offset, CheepsPerPage);
        }

        return cheeps;
    }

    /// <summary>
    /// Retrieves cheeps for a user's timeline, including their own cheeps
    /// and cheeps from authors they follow.
    /// </summary>
    /// <param name="userName">Username of the timeline owner.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <returns>
    /// A list of cheeps for the user's timeline.
    /// </returns>
    public async Task<List<CheepDTO>> GetUserTimelineCheeps(string user ,string authPage, int pageNumber)
    {
        List<string> userNames = new List<string>();
        int offset = (pageNumber - 1) * CheepsPerPage;
        if (!authPage.Equals(user)) // /andenPersonsTimeline
        {
            userNames.Add(authPage);
        } else if (authPage.Equals(user)) // hvis vi er inde på egen timeline
        {
            userNames.Add(user);
            List<AuthorDTO> following = await _authorService.GetFollowing(user);
            foreach (var author in following)
            {
                userNames.Add(author.Name);
            }
        }

        List<CheepDTO> cheeps = await _cheepRepository.ReadCheepsFromFollowers(userNames, offset, CheepsPerPage);
        return cheeps;
    }

    /// <summary>
    /// Retrieves cheeps saved by a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <returns>
    /// A list of saved cheeps.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user does not exist.
    /// </exception>
    public async Task<List<CheepDTO>> GetSavedCheeps(string userName, int pageNumber)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null) throw new InvalidOperationException("User: " + userName + " doesn't exist");

        int offset = (pageNumber - 1) * CheepsPerPage;

        List<CheepDTO> savedCheeps = await _cheepRepository.ReadSavedCheeps(author.AuthorId, offset, CheepsPerPage);

        return savedCheeps;
    }

    /// <summary>
    /// Creates a new cheep for a specific user.
    /// </summary>
    /// <param name="userName">Username of the author.</param>
    /// <param name="text">Text content of the cheep.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user does not exist.
    /// </exception>
    public async Task CreateCheepForUser(string userName, string text)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException("user with username: " + userName + " doesn't exist");
        }

        CheepDTO newCheep = new CheepDTO
        {
            Author = author,
            Text = text,
            TimeStamp = DateTime.Now
        };

        await _cheepRepository.CreateCheep(newCheep);
    }

    /// <summary>
    /// Saves a cheep for a user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="cheepId">ID of the cheep to save.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user or cheep does not exist.
    /// </exception>
    public async Task SaveCheepForUser(string userName, long cheepId)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException("user with username: " + userName + " doesn't exist");
        }

        CheepDTO? cheep = await _cheepRepository.GetCheepById(cheepId);
        if (cheep == null)
        {
            throw new InvalidOperationException($"Cheep with id " + cheepId + " doesn't exist");
        }

        await _cheepRepository.SaveCheep(author, cheep);
    }

    /// <summary>
    /// Removes a saved cheep for a user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="cheepId">ID of the cheep to remove.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user or cheep does not exist.
    /// </exception>
    public async Task RemoveSavedCheepForUser(string userName, long cheepId)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException("user with username: " + userName + " doesn't exist");
        }

        CheepDTO? cheep = await _cheepRepository.GetCheepById(cheepId);
        if (cheep == null)
        {
            throw new InvalidOperationException($"Cheep with id " + cheepId + " doesn't exist");
        }

        await _cheepRepository.RemoveSavedCheep(author, cheep);
    }

    /// <summary>
    /// Determines whether a cheep is saved by a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <param name="cheepId">ID of the cheep.</param>
    /// <returns>
    /// <c>true</c> if the cheep is saved by the user; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user or cheep does not exist.
    /// </exception>
    public async Task<bool> IsCheepSavedByUser(string userName, long cheepId)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException("user with username: " + userName + " doesn't exist");
        }
        CheepDTO? cheep = await _cheepRepository.GetCheepById(cheepId);
        if (cheep == null)
        {
            throw new InvalidOperationException("Cheep with id " + cheepId + " doesn't exist");
        }

        bool isSaved = await _cheepRepository.IsSaved(author, cheep);
        return isSaved;
    }

    /// <summary>
    /// Deletes all saved cheeps for a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user does not exist.
    /// </exception>
    public async Task DeleteAllSavedCheepsForUser(string userName)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException("user with username: " + userName + " doesn't exist");
        }
        await _cheepRepository.DeleteSavedCheeps(userName);
    }

    /// <summary>
    /// Deletes all cheeps authored by a specific user.
    /// </summary>
    /// <param name="userName">Username of the user.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the user does not exist.
    /// </exception>
    public async Task DeleteAllCheepsForUser(string userName)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException("user with username: " + userName + " doesn't exist");
        }
        await _cheepRepository.DeleteCheeps(userName);
    }




}