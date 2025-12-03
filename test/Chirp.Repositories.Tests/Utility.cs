using Chirp.Core;

namespace Chirp.Repositories.Tests;

public class Utility
{
    static private HashSet<string> _usedAuthorNames = new HashSet<string>();
    static private int _counter = 200;

    static public void resetUsernames()
    {
        _usedAuthorNames.Clear();
    }
    static public string RandomString(int length)
    {
        var generatedString = new char[length];
        for (var i = 0; i < length; i++)
        {
            generatedString[i] = (char)new Random().Next(33, 127);
        }
        return new string(generatedString);
    }

    static public Author RandomTestUser(bool unique)
    {
        if (unique && _usedAuthorNames == null) throw new NullReferenceException("_usedAuthorNames is null");
        var username = RandomString(new Random().Next(1, 25));
        if(unique) {
            while (_usedAuthorNames.Contains(username))
            {
                username = RandomString(new Random().Next(1, 25));
            }
            _usedAuthorNames.Add(username);
        }


        return new Author()
        {
            Id = _counter++,
            UserName = username,
            Email = username + "@hotmail.com",
            Cheeps = new List<Cheep>()
        };
    }
}