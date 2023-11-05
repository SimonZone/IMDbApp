using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDbApp.Titles
{
    internal class TitleWithActor
    {
        public TitleWithActor(
            string primaryTitle, string name, string category, string character)
        {
            this.primaryTitle = primaryTitle;
            this.name = name;
            this.category = category;
            this.character = character;
        }
        public string primaryTitle { get; set; }
        public string name { get; set; }
        public string category { get; set; }
        public string character { get; set; }


        public override string ToString()
        {
            return $"Title: {primaryTitle}: Name of actor: {name},category of work: {category}.";
        }
    }
}
