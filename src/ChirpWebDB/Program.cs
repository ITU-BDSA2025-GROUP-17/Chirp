using simpleDB;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
CSVDatabase<Cheep> db = new CSVDatabase<Cheep>("chirp_db.csv");

app.MapGet("/cheeps", () =>
{
    return db.Read();
});

app.MapPost("/cheep", (Cheep cheep) =>
{
    db.Store(cheep);
    return "given cheep: "+cheep;
});

app.Run();

public record Cheep(string Author, string Message, long Timestamp);