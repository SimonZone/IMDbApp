using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMDbApp.Titles
{
    internal class Title
    {
        public Title(int titleID, string titleType,
            string primaryTitle, string originalTitle,
            bool isAdult, int? startYear,
            int? endYear, int? runtimeMinutes)
        {
            this.titleID = titleID;
            this.titleType = titleType;
            this.primaryTitle = primaryTitle;
            this.originalTitle = originalTitle;
            this.isAdult = isAdult;
            this.startYear = startYear;
            this.endYear = endYear;
            this.runtimeMinutes = runtimeMinutes;
        }

        public int titleID { get; set; }
        public string titleType { get; set; }
        public string primaryTitle { get; set; }
        public string originalTitle { get; set; }
        public bool isAdult { get; set; }
        public int? startYear { get; set; }
        public int? endYear { get; set; }
        public int? runtimeMinutes { get; set; }

        public override string ToString()
        {
            return $"titleID: {titleID}: titleType: {titleType}, \n" +
                   $"primaryTitle: {primaryTitle}, \n" +
                   $"originalTitle: {originalTitle}, \n" +
                   $"isAdult: {isAdult}, \n" +
                   $"startYear: {startYear}, endYear: {endYear} \n" +
                   $"runtimeMinutes: {runtimeMinutes}\n";
        }
    }
}
