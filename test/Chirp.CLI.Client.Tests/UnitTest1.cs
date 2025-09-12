namespace Chirp.CLI.Client.Tests;

using Chirp.CLI;

public class UnitTest1
{
    [Fact]
    public void T()
    {

    }

    [Fact]
    public void TestReadTenCheeps()
    {
        // Arrange
        var args = new string[] { "read", "10" };
        // Act
        Program.Main(args);
        // Assert
        Assert.Equal(0, result);
    }
}
