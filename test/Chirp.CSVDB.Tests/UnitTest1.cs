namespace Chirp.CLI.Client.Tests;

using System.Security.Principal;
using Chirp.CLI;
using simpleDB;

public class UnitTest1
{

    [Fact]
    public void TestReadTenCheeps()
    {
        // Arrange
        var correct = new List<Cheep>(
            [
                new Cheep("ropf","Hello, BDSA students!",1690891760),
                new Cheep("adho","Welcome to the course!",1690978778),
                new Cheep("adho","I hope you had a good summer.",1690979858),
                new Cheep("ropf","Cheeping cheeps on Chirp :)",1690981487)
            ]
        );
        // Act
        CSVDatabase<Cheep> db = new CSVDatabase<Cheep>("../../../test_read_db.csv");
        List<Cheep> result = db.Read(10).ToList();

        // Assert
        for(int i = 0; i < result.Count; i++) {
            Assert.Equal(correct[i], result[i]);
        }
    }
}
