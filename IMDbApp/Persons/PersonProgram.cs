using System.Data.SqlClient;

namespace IMDbApp.Persons
{
    internal class PersonProgram
    {
        string? query;
        string? searchQuery;
        List<Person> personList = new();
        int currentPage = 0;
        bool exit = false;

        public void Run(string connString)
        {
            SqlConnection sqlConn = new(connString);
            sqlConn.Open();

            Console.Clear();
            Console.WriteLine("Welcome to the person section,");
            Console.WriteLine();
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Search for persons");
            Console.WriteLine("2. Add a person");
            Console.WriteLine("3. Get persons with theirs titles");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    currentPage = 0;
                    Console.WriteLine("What person do you want to search for?");
                    searchQuery = Console.ReadLine();

                    Console.WriteLine("getting persons");
                    GetPersons(searchQuery!, currentPage, sqlConn);
                    exit = false;
                    while (!exit)
                    {
                        Console.WriteLine("Page: " + currentPage);
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.RightArrow:
                                GetPersons(searchQuery!, currentPage++, sqlConn);
                                break;
                            case ConsoleKey.LeftArrow:
                                if (currentPage >= 1)
                                {
                                    GetPersons(searchQuery!, currentPage--, sqlConn);
                                }
                                break;
                            case ConsoleKey.X:
                                exit = true;
                                break;
                            default:
                                Console.WriteLine("you did not press a supported key");
                                break;
                        }
                    }
                    break;  // Get persons
                case "2":
                    AddPerson(sqlConn);
                    break;  // Add person
                case "3":
                    currentPage = 0;
                    Console.WriteLine("What person do you want to search for?");
                    searchQuery = Console.ReadLine();

                    Console.WriteLine("getting person with titles");
                    GetPersonsWithTitles(searchQuery!, currentPage, sqlConn);
                    exit = false;
                    while (!exit)
                    {
                        Console.WriteLine("Page: " + currentPage);
                        ConsoleKeyInfo keyInfo = Console.ReadKey();
                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.RightArrow:
                                GetPersonsWithTitles(searchQuery!, currentPage++, sqlConn);
                                break;
                            case ConsoleKey.LeftArrow:
                                if (currentPage >= 1)
                                {
                                    GetPersonsWithTitles(searchQuery!, currentPage--, sqlConn);
                                }
                                break;
                            case ConsoleKey.X:
                                exit = true;
                                break;
                            default:
                                Console.WriteLine("you did not press a supported key");
                                break;
                        }
                    }
                    break;  // Get persons with theirs titles
                default:
                    break;
            }
            sqlConn.Close();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
        public void GetPersons(string searchQuery, int page, SqlConnection sqlConn)
        {
            personList.Clear();
            query = $"EXECUTE [dbo].[GetPersons] @SearchQuery, @Page";
            personList = new List<Person>();
            using SqlCommand cmd = new(query, sqlConn);
            cmd.CommandTimeout = 120;
            cmd.Parameters.AddWithValue("@SearchQuery", searchQuery);
            cmd.Parameters.AddWithValue("@Page", page);
            // sends request and reads the persons from the database
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Person newPerson = new(
                    ConvertToId(reader["personID"].ToString()!),
                    reader["name"].ToString()!,
                    ConvertToInt(reader["birthYear"].ToString()!),
                    ConvertToInt(reader["deathYear"].ToString()!)
                    );
                personList.Add(newPerson);
            }


            foreach (var person in personList)
            {
                Console.WriteLine(person);
            }

            Console.WriteLine("Press right arrow to get the next 10 titles or left arrow to see the last 10 titles");
            Console.WriteLine("Or Press X to exit");
        }
        public void AddPerson(SqlConnection sqlConn)
        {
            personList.Clear();
            Console.WriteLine("Lets go over what i need to add a person!");
            Console.WriteLine("\nWhat is the name of the person?");
            string? name = Console.ReadLine();
            Console.WriteLine("\nwhat year was they born?");
            string? birthYear = Console.ReadLine();
            Console.WriteLine("\nIf they are dead, what year did they die? optional");
            string? deathYear = Console.ReadLine();

            query = "EXECUTE [dbo].[AddPerson] @name ,@birthYear, @deathYear";

            // prepares the data to be added
            using SqlCommand cmd = new(query, sqlConn);
            cmd.Parameters.AddWithValue("@name", HandleStringIfEmpty(name!) ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@birthYear", birthYear! ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@deathYear", deathYear! ?? (object)DBNull.Value);

            // sends data and reads the person added from the database
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Person newPerson = new(
                    ConvertToId(reader["personID"].ToString()!),
                    reader["name"].ToString()!,
                    ConvertToInt(reader["birthYear"].ToString()!),
                    ConvertToInt(reader["deathYear"].ToString()!)
                    );
                personList.Add(newPerson);
            }
            // shows the persons
            foreach (var person in personList)
            {
                Console.WriteLine(person);
            }
        }
        public void GetPersonsWithTitles(string searchQuery, int page, SqlConnection sqlConn)
        {
            query = $"EXECUTE [dbo].[GetPersonsWithTitles] @SearchQuery, @Page";
            List<PersonsTitles> personsTitles = new();

            // sends request and reads the persons with title info from the database
            using (SqlCommand cmd = new(query, sqlConn))
            {
                cmd.Parameters.AddWithValue("@SearchQuery", searchQuery);
                cmd.Parameters.AddWithValue("@Page", page);
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    PersonsTitles newPersonsTitles = new(
                        reader["name"].ToString()!,
                        reader["category"].ToString()!,
                        reader["primaryTitle"].ToString()!,
                        reader["character"].ToString()!
                        );
                    personsTitles.Add(newPersonsTitles);
                }
            }

            // formats the person with titles
            var groupedPersons = personsTitles.GroupBy(m => m.name);
            string principalString;
            foreach (var group in groupedPersons)
            {
                Console.WriteLine($"Name: {group.Key}");
                foreach (var person in group)
                {
                    if (ConvertToString(person.character) == null)
                    {
                        principalString = $"Category of work: {person.category} in title: {person.primaryTitle}";
                        Console.WriteLine(principalString);
                    }
                    else
                    {
                        principalString = $"Category of work: {person.category}, as {person.character} in title: {person.primaryTitle}";
                        Console.WriteLine(principalString);
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine("Press right arrow to get the next 10 titles or left arrow to see the last 10 titles");
            Console.WriteLine("Or Press X to exit");
        }

        #region  Converters
        static int ConvertToId(string input)
        {
            return int.Parse(input);
        }

        static int? ConvertToInt(string input)
        {
            if (input == null || input == "")
            {
                return null;
            }
            return int.Parse(input);
        }

        static string? ConvertToString(string input)
        {
            if (input.ToLower() == @"\n")
            {
                return null;
            }
            return input;
        }
        static string? HandleStringIfEmpty(string input)
        {
            if (input == "") return null; else return input;
        }

        #endregion
    }
}
