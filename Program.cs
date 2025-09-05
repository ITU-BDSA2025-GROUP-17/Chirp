using System.Globalization;
using Chirp.CLI;
using DocoptNet;
using simpleDB;

CSVDatabase<Cheep> database = new("chirp_cli_db.csv");

const string usage = @"Chirp CLI version.

Usage:
  chirp read
  chirp read <limit>
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";

IDictionary<string, ValueObject> arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

if (arguments["read"].IsTrue) {
  if(!arguments["<limit>"].IsInt) UserInterface.PrintCheeps(database.Read());
  else UserInterface.PrintCheeps(database.Read(arguments["<limit>"].AsInt));
  
}else if (arguments["cheep"].IsTrue)
{
    Cheep(arguments["<message>"].ToString());
}

void Cheep(string message)
{
    string name = Environment.UserName;
    long time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    database.Store(new Cheep(name, message, time));
}
