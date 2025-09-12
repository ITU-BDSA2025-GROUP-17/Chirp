namespace Chirp.CLI.Client.Tests;

using Chirp.CLI;
using simpleDB;

public class UnitTest1
{

    [Fact]
    public void TestReadTenCheeps()
    {
        // Arrange
        var args = new string[] { "read", "10" };
        // Act
        CSVDatabase<Cheep> db = new CSVDatabase<Cheep>("../../../test_read_db.csv");
        List<Cheep> db_result = db.Read(10).ToList();
        string result = UserInterface.FormatCheeps(db_result);
        // Assert
        Assert.Equal("ropf @ 01-08-2023 14:09:20: Hello, BDSA students!\nadho @ 02-08-2023 14:19:38: Welcome to the course!\nadho @ 02-08-2023 14:37:38: I hope you had a good summer.\nropf @ 02-08-2023 15:04:47: Cheeping cheeps on Chirp :)\n", result);
    }
}
