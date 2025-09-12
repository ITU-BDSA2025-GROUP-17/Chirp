namespace Chirp.CLI.Client.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

    }

    [Fact]
    public void TestReadTenCheeps()
    {
        // Arrange
        var args = new string[] { "read", "10" };
        // Act
        var result = Program.Main(args);
        // Assert
        Assert.Equal(0, result);
    }
}
