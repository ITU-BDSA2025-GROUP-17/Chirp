namespace Chirp.Services;

using Repositories;
public class CheepService : ICheepService
{
    private readonly ICheepRepository _cheepRepository;

    private readonly IAuthorService _authorService;

    private const int CheepsPerPage = 32;

    public CheepService(ICheepRepository cheepRepository, IAuthorService authorService)
    {
        _cheepRepository = cheepRepository;
        _authorService = authorService;
    }

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
    public async Task<List<CheepDTO>> GetUserTimelineCheeps(string userName, int pageNumber)
    {
        int offset = (pageNumber - 1) * CheepsPerPage;

        //list of string, userNames
        List<AuthorDTO> following = await _authorService.GetFollowing(userName);

        List<string> userNames = new List<string> { userName };
        foreach (var author in following)
        {
            userNames.Add(author.Name);
        }


        List<CheepDTO> cheeps = await _cheepRepository.ReadCheepsFromFollowers(userNames, offset, CheepsPerPage);
        return cheeps;
    }
    public async Task<List<CheepDTO>> GetSavedCheeps(string userName, int pageNumber)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null) throw new InvalidOperationException("User: " + userName + " doesn't exist");

        int offset = (pageNumber - 1) * CheepsPerPage;

        List<CheepDTO> savedCheeps = await _cheepRepository.ReadSavedCheeps(author.AuthorId, offset, CheepsPerPage);

        return savedCheeps;
    }
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
    public async Task DeleteAllSavedCheepsForUser(string userName)
    {
        AuthorDTO? author = await _authorService.GetAuthorByName(userName);
        if (author == null)
        {
            throw new InvalidOperationException("user with username: " + userName + " doesn't exist");
        }
        await _cheepRepository.DeleteSavedCheeps(userName);
    }
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