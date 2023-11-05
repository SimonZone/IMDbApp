using System.Data.SqlClient;

namespace IMDbApp.Persons
{
    internal class PersonProgram
    {
        string? query;
        string? searchQuery;
        List<Person> personList = new();
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
                    Console.WriteLine("What person do you want to search for?");
                    searchQuery = Console.ReadLine();

                    Console.WriteLine("getting persons");
                    GetPersons(searchQuery!, sqlConn);
                    break;  // Search for persons
                case "2":
                    AddPerson(sqlConn);
                    break;
                case "3":
                    Console.WriteLine("What person do you want to search for?");
                    searchQuery = Console.ReadLine();

                    Console.WriteLine("getting person with titles");
                    GetActorsTitles(searchQuery!, sqlConn);
                    break;  // Get persons with theirs titles
                default:
                    break;
            }
            sqlConn.Close();
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
        public void GetPersons(string searchQuery, SqlConnection sqlConn)
        {
            personList.Clear();
            query = $"EXECUTE [dbo].[WildcardSearchingPersons] '{searchQuery}'";
            personList = new List<Person>();
            using SqlCommand cmd = new(query, sqlConn);

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
        public void GetActorsTitles(string searchQuery, SqlConnection sqlConn)
        {
            query = $"EXECUTE [dbo].[GetActorWithMovies] '{searchQuery}'";
            List<PersonsTitles> personsTitles = new();

            // sends request and reads the persons with title info from the database
            using (SqlCommand cmd = new(query, sqlConn))
            {
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
