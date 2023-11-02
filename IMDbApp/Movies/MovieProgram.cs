using System.Data.SqlClient;

namespace IMDbApp.Movies
{
    internal class MovieProgram
    {
        string? query;
        List<Movie> movieList = new List<Movie>();
        public void Run(string connString)
        {
            SqlConnection sqlConn = new(connString);
            sqlConn.Open();
            SqlCommand cmd;
            Console.Clear();
            Console.WriteLine("Welcome to the title section,");
            Console.WriteLine();
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Search for titles");
            Console.WriteLine("2. Add a title");
            Console.WriteLine("3. Edit a title");
            Console.WriteLine("4. Delete a title");
            Console.WriteLine("5. Get top 50");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("What movie name do you want to search for?");
                    string? searchQuery = Console.ReadLine();

                    Console.WriteLine("getting movies");
                    GetMovies(searchQuery!, sqlConn);
                    foreach (var movie in movieList)
                    {
                        Console.WriteLine(movie);
                    }
                    break;
                case "2":
                    Console.WriteLine("Lets go over what i need to add a title!");
                    Console.WriteLine("\nWhat type of title is it?");
                    string? titleType = Console.ReadLine();
                    Console.WriteLine("\nWhat is the primary title of the title?");
                    string? primaryTitle = Console.ReadLine();
                    Console.WriteLine("\nWhat is the original title of the title?");
                    string? originalTitle = Console.ReadLine();
                    Console.WriteLine("\nIs title for adults only?");
                    string? isAdult = Console.ReadLine();
                    Console.WriteLine("\nWhat year was the title released? This one is optional");
                    string? startYear = Console.ReadLine();
                    Console.WriteLine("\nWhat year did it the title end? This one is optional");
                    string? endYear = Console.ReadLine();
                    Console.WriteLine("\nHow long in minutes is the title This one is optional");
                    string? runtimeMinutes = Console.ReadLine();


                    cmd = new($"EXECUTE [dbo].[AddMovieTitle] " +
                        $"'{titleType}'" +
                        $",'{primaryTitle}'" +
                        $",'{originalTitle}'" +
                        $",{ConvertToBool(isAdult!)}" +
                        $",{ConvertToInt(startYear!)}" +
                        $",{ConvertToInt(endYear!)}" +
                        $",{ConvertToInt(runtimeMinutes!)}"
                        , sqlConn);
                    Console.WriteLine("Deleting movie");
                    cmd.ExecuteNonQuery();
                    break;
                case "3":

                    break;
                case "4":
                    Console.WriteLine("What movie do you want to Delete?");
                    string? deleteQuery = Console.ReadLine();

                    if (int.TryParse(deleteQuery,out int result))
                    {
                        cmd = new($"EXECUTE[dbo].[DeleteMovieTitle] {result}", sqlConn);
                        Console.WriteLine("Deleting movie");
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        Console.WriteLine("You did not enter a id as a number");
                    }
                    break;
                case "5":

                    Console.WriteLine("getting movies");
                    GetTop50Movies(sqlConn);
                    foreach (var movie in movieList)
                    {
                        Console.WriteLine(movie);
                        Console.WriteLine("Cool movie :)");
                    }
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
            query = $"EXECUTE [dbo].[WildcardSearchingMovies] '{searchQuery}'";
            movieList = new List<Movie>();
            using (SqlCommand cmd = new SqlCommand(query, sqlConn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Movie newMovie = new(
                            ConvertToId(reader["titleID"].ToString()!),
                            reader["titleType"].ToString()!,
                            reader["primaryTitle"].ToString()!,
                            reader["originalTitle"].ToString()!,
                            ConvertToCBool(reader["isAdult"].ToString()!),
                            ConvertToInt(reader["startYear"].ToString()!),
                            ConvertToInt(reader["endYear"].ToString()!),
                            ConvertToInt(reader["runtimeMinutes"].ToString()!)
                            );
                        movieList.Add(newMovie);
                    }
                }
            }
        }
        public void GetTop50Movies(SqlConnection sqlConn)
        {
            query = "SELECT TOP (50) [titleID], [titleType], [primaryTitle], [originalTitle], [isAdult], [startYear], [endYear], [runtimeMinutes] FROM [IMDb].[dbo].[TitleBasics]";
            movieList = new List<Movie>();
            using (SqlCommand cmd = new SqlCommand(query, sqlConn))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine(reader.ToString());
                        Movie newMovie = new(
                            ConvertToId(reader["titleID"].ToString()!),
                            reader["titleType"].ToString()!,
                            reader["primaryTitle"].ToString()!,
                            reader["originalTitle"].ToString()!,
                            ConvertToBool(reader["isAdult"].ToString()!),
                            ConvertToInt(reader["startYear"].ToString()!),
                            ConvertToInt(reader["endYear"].ToString()!),
                            ConvertToInt(reader["runtimeMinutes"].ToString()!)
                            );

                        movieList.Add(newMovie);
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

        bool ConvertToCBool(string input)
        {
            bool inputAsBool = bool.Parse(input);
            return inputAsBool;
        }
        bool ConvertToBool(string input)
        {
            int inputInt = int.Parse(input);
            if (inputInt == 0) return false; else return true;
            throw new ArgumentException("Could not convert to bool");
        }
    }
}
