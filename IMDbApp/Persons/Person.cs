namespace IMDbApp.Persons
{
    internal class Person
    {
        public Person(int personId, string name,
            int? birthYear, int? deathYear)
        {
            this.personID = personId;
            this.name = name;
            this.birthYear = birthYear;
            this.deathYear = deathYear;
        }

        public int personID { get; set; }
        public string name { get; set; }
        public int? birthYear { get; set; }
        public int? deathYear { get; set; }


        public override string ToString()
        {
            return $"personID: {personID}: name: {name}, \n" +
                   $"birthYear: {birthYear}, deathYear: {deathYear} \n";
        }
    }
}
