using IMDbApp.Movies;
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
        List<Person> personList = new List<Person>();
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
            Console.WriteLine("3. Edit a person");
            Console.WriteLine("4. Delete a person");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("What person name do you want to search for?");
                    string? searchQuery = Console.ReadLine();

                    Console.WriteLine("getting persons");
                    GetMovies(searchQuery!, sqlConn);
                    foreach (var person in personList)
                    {
                        Console.WriteLine(person);
                    }
                    break;
                case "2":

                    break;
                case "3":

                    break;
                case "4":

                    break;
                default:
                    break;
            }
            sqlConn.Close();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
        public void GetMovies(string searchQuery, SqlConnection sqlConn)
        {
            query = $"EXECUTE [dbo].[WildcardSearchingPersons] '{searchQuery}'";
            personList = new List<Person>();
            using (SqlCommand cmd = new SqlCommand(query, sqlConn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
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
            }
        }
        int ConvertToId(string input)
        {
            return int.Parse(input);
        }
        int? ConvertToInt(string input)
        {
            if (input == null || input == "")
            {
                return null;
            }
            return int.Parse(input);
        }
    }
}
