using IMDbApp.Titles;
using IMDbApp.Persons;

string connString = "server=localhost;database=IMDb;" +
    "user id=sa;password=Detstores123!;TrustServerCertificate=True";

TitleProgram titleProgram = new();
PersonProgram personProgram = new();

while (true)
{
    Console.Clear();

    Console.WriteLine("Hello Welcome to IMDb app for viewing and modifying titles and related things.");
    Console.WriteLine();
    Console.WriteLine("What do you want to do?");
    Console.WriteLine("1. See options for Titles");
    Console.WriteLine("2. See options for People");
    Console.WriteLine("3. Exit");
    string? input = Console.ReadLine();

    switch (input)
    {
        case "1":
            titleProgram.Run(connString);
            break;
        case "2":
            personProgram.Run(connString);
            break;
        case "3":
            Console.WriteLine("Exiting");
            return;
        default:
            break;
    }
}