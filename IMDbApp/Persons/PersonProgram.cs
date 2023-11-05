using IMDbApp.Titles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Console.WriteLine("2. Add a person, not working");
            Console.WriteLine("3. Get persons with theirs titles");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("What person do you want to search for?");
                    searchQuery = Console.ReadLine();

                    Console.WriteLine("getting persons");
                    GetTitles(searchQuery!, sqlConn);
                    foreach (var person in personList)
                    {
                        Console.WriteLine(person);
                    }
                    break;  // Search for persons
                case "2":

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
        public void GetTitles(string searchQuery, SqlConnection sqlConn)
        {
            query = $"EXECUTE [dbo].[WildcardSearchingPersons] '{searchQuery}'";
            personList = new List<Person>();
            using SqlCommand cmd = new(query, sqlConn);
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
        }

        public void GetActorsTitles(string searchQuery, SqlConnection sqlConn)
        {
            query = $"EXECUTE [dbo].[GetActorWithMovies] '{searchQuery}'";
            List<PersonsTitles> personsTitles = new();
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
        #endregion

    }
}
