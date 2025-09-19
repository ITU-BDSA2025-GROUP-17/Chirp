namespace Chirp.CLI.Client.Tests;
// Chirp.CVSBD tests for the database


using System.Security.Principal;
using Chirp.CLI;
using simpleDB;
using System;
using System.IO;
using System.Linq;
using Xunit;

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
        for (int i = 0; i < result.Count; i++)
        {
            Assert.Equal(correct[i], result[i]);
        }
    }

    public class UltraSimpleTest
    {
        [Fact]
        public void StoreAndReadOneCheep()
        {
            //temp empty CSV file
            var path = Path.Combine(Path.GetTempPath(), $"chirp_{Guid.NewGuid():N}.csv");
            File.WriteAllText(path, string.Empty);

            //makes a db and can read Cheeps
            var db = new CSVDatabase<Cheep>(path);
            var expected = new Cheep("Jaden", "Hej", 123);

            db.Store(expected);
            var actual = db.Read(1).First();

            Assert.Equal(expected, actual);
        }
    }
}
