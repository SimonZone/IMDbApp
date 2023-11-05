using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;

namespace IMDbApp.Titles
{
    internal class TitleProgram
    {
        string? searchQuery;
        string? query;
        List<Title> titleList = new();
        public void Run(string connString)
        {
            SqlConnection sqlConn = new(connString);
            sqlConn.Open();
            Console.Clear();
            Console.WriteLine("Welcome to the title section,");
            Console.WriteLine();
            Console.WriteLine("What do you want to do?");
            Console.WriteLine("1. Search for titles");
            Console.WriteLine("2. Add a title");
            Console.WriteLine("3. Edit a title");
            Console.WriteLine("4. Delete a title");
            Console.WriteLine("5. Get title with its actors");
            string? input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("What title name do you want to search for?");
                    searchQuery = Console.ReadLine();

                    Console.WriteLine("getting titles");
                    GetTitles(searchQuery!, sqlConn);
                    foreach (var title in titleList)
                    {
                        Console.WriteLine(title);
                    }
                    break;      // Search for title
                case "2":
                    AddTitle(sqlConn);
                    break;      // Add a title
                case "3":         
                    UpdateTitle(sqlConn);
                    foreach (var title in titleList)
                    {
                        Console.WriteLine(title);
                    }
                        break;      // Edit a title
                case "4":
                    Console.WriteLine("What title do you want to delete? Enter the title's id:");
                    searchQuery = Console.ReadLine();
                    Title? deletedTitle = DeleteAndReturnTitle(searchQuery, sqlConn);
                    if (deletedTitle == null)
                    {
                        Console.WriteLine("You did not enter a number or no title with that id was deleted");
                    }
                    else
                    {
                        Console.WriteLine(deletedTitle);
                    }
                    break;      // Delete a title
                case "5":         
                    Console.WriteLine("What title name do you want to search for?");
                    searchQuery = Console.ReadLine()!;

                    Console.WriteLine("getting titles");
                    GetTitlesWithActors(searchQuery, sqlConn);
                    break;      // Get title with its actors
                default:
                    break;
            }
            sqlConn.Close();

            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
        }
        public void GetTitlesWithActors(string searchQuery, SqlConnection sqlConn)
        {
            query = $"EXECUTE [dbo].[GetMovieWithActors] '{searchQuery}'";
            List<TitleWithActor> titlesWithActors = new();
            using (SqlCommand cmd = new(query, sqlConn))
            {
                using SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TitleWithActor newTitleWithActor = new(
                        reader["primaryTitle"].ToString()!,
                        reader["name"].ToString()!,
                        reader["category"].ToString()!,
                        reader["character"].ToString()!
                        );
                    titlesWithActors.Add(newTitleWithActor);
                }
            }
            var groupedTitles = titlesWithActors.GroupBy(m => m.primaryTitle);
            string principalString;
            foreach (var group in groupedTitles)
            {
                Console.WriteLine($"Title: {group.Key}");
                foreach (var title in group)
                {
                    if (ConvertToString(title.character) == null)
                    {
                        principalString = $"Name of person: {title.name}, category of work: {title.category}.";
                        Console.WriteLine(principalString);
                    }
                    else
                    {
                        principalString = $"Character: {title.character} played by: {title.name}, category of work: {title.category}.";
                        Console.WriteLine(principalString);
                    }
                }
                Console.WriteLine();
            }
        }
        public static void AddTitle(SqlConnection sqlConn)
        {
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

            SqlCommand cmd;

            cmd = new($"EXECUTE [dbo].[AddMovieTitle] " +
                $"'{titleType}'" +
                $",'{primaryTitle}'" +
                $",'{originalTitle}'" +
                $",{ConvertToBool(isAdult!)}" +
                $",{ConvertToInt(startYear!)}" +
                $",{ConvertToInt(endYear!)}" +
                $",{ConvertToInt(runtimeMinutes!)}"
                , sqlConn);
            Console.WriteLine("Deleting title");
            cmd.ExecuteNonQuery();
        }
        public void UpdateTitle(SqlConnection sqlConn)
        {
            titleList.Clear();

            Console.WriteLine("Lets go over what i need to update a title!");
            Console.WriteLine("\nWhat is the id of the title to be updated?");
            string? titleID = Console.ReadLine();
            Console.WriteLine("\nWhat is the new type of title is it? optional");
            string? titleType = Console.ReadLine();
            Console.WriteLine("\nWhat is the new primary title of the title? optional");
            string? primaryTitle = Console.ReadLine();
            Console.WriteLine("\nWhat is the new original title of the title? optional");
            string? originalTitle = Console.ReadLine();
            Console.WriteLine("\nIs title for adults only? type 1 for yes and 0 for no or nothing to skip. optional");
            string? isAdult = Console.ReadLine();
            Console.WriteLine("\nWhat is the new value for what year the title was released? optional");
            string? startYear = Console.ReadLine();
            Console.WriteLine("\nWhat is the new value for What year the title ended? optional");
            string? endYear = Console.ReadLine();
            Console.WriteLine("\nWhat is the new value How long in minutes the title is? optional");
            string? runtimeMinutes = Console.ReadLine();

            query = "EXECUTE [dbo].[UpdateMovieTitleWithBeforeAndAfter] " +
                "@titleID, @titleType, @primaryTitle, @originalTitle, @isAdult, @startYear, @endYear, @runtimeMinutes";


            using SqlCommand cmd = new(query, sqlConn);
            if (titleID != null)
            {
                cmd.Parameters.AddWithValue("@titleID", ConvertToInt(titleID!));
                cmd.Parameters.AddWithValue("@titleType", HandleStringIfEmpty(titleType!) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@primaryTitle", HandleStringIfEmpty(primaryTitle!) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@originalTitle", HandleStringIfEmpty(originalTitle!) ?? (object)DBNull.Value);
                if (isAdult == null)
                {
                    cmd.Parameters.AddWithValue("@isAdult", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@isAdult", ConvertToBool(isAdult)!);
                }
                cmd.Parameters.AddWithValue("@startYear", ConvertToInt(startYear!) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@endYear", ConvertToInt(endYear!) ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@runtimeMinutes", ConvertToInt(runtimeMinutes!) ?? (object)DBNull.Value);
            }
            else
            {
                Console.WriteLine("You did not enter a valid id");
                return;
            }

            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {

                Title newTitle = new(
                    ConvertToId(reader["titleID"].ToString()!),
                    reader["titleType"].ToString()!,
                    reader["primaryTitle"].ToString()!,
                    reader["originalTitle"].ToString()!,
                    ConvertToCBool(reader["isAdult"].ToString()!),
                    ConvertToInt(reader["startYear"].ToString()!),
                    ConvertToInt(reader["endYear"].ToString()!),
                    ConvertToInt(reader["runtimeMinutes"].ToString()!)
                    );
                titleList.Add(newTitle);
            }
        }
        public static Title? DeleteAndReturnTitle(string? deleteQuery, SqlConnection sqlConn)
        {
            if (int.TryParse(deleteQuery, out int titleID) == false) return null;
            Title deletedTitle = null!;

            using (SqlCommand cmd = new("DeleteAndReturnMovie", sqlConn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@titleID", SqlDbType.Int) { Value = titleID });

                using SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    deletedTitle = new Title(
                        ConvertToId(reader["titleID"].ToString()!),
                        reader["titleType"].ToString()!,
                        reader["primaryTitle"].ToString()!,
                        reader["originalTitle"].ToString()!,
                        ConvertToBool(reader["isAdult"].ToString()!),
                        ConvertToInt(reader["startYear"].ToString()!),
                        ConvertToInt(reader["endYear"].ToString()!),
                        ConvertToInt(reader["runtimeMinutes"].ToString()!)
                    );
                }
            }
            return deletedTitle;
        }
        public void GetTitles(string searchQuery, SqlConnection sqlConn)
        {
            query = $"EXECUTE [dbo].[WildcardSearchingMovies] '{searchQuery}'";
            titleList = new List<Title>();
            using SqlCommand cmd = new(query, sqlConn);
            using SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Title newTitle = new(
                    ConvertToId(reader["titleID"].ToString()!),
                    reader["titleType"].ToString()!,
                    reader["primaryTitle"].ToString()!,
                    reader["originalTitle"].ToString()!,
                    ConvertToCBool(reader["isAdult"].ToString()!),
                    ConvertToInt(reader["startYear"].ToString()!),
                    ConvertToInt(reader["endYear"].ToString()!),
                    ConvertToInt(reader["runtimeMinutes"].ToString()!)
                    );
                titleList.Add(newTitle);
            }
        }

        #region Converters
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

        static bool ConvertToCBool(string input)
        {
            bool inputAsBool = bool.Parse(input);
            return inputAsBool;
        }

        static bool ConvertToBool(string input)
        {
            if (input == "False" || input == "0") return false; else return true;
        }

        static string? HandleStringIfEmpty(string input)
        {
            if (input == "") return null; else return input;
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
