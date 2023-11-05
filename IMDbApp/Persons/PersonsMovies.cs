namespace IMDbApp.Persons
{
    internal class PersonsTitles
    {
        public PersonsTitles(string name,
            string category, string primaryTitle,
            string character)
        {
            this.name = name;
            this.category = category;
            this.primaryTitle = primaryTitle;
            this.character = character;
        }
        public string name { get; set; }
        public string category { get; set; }
        public string primaryTitle { get; set; }
        public string character { get; set; }
    }
}
