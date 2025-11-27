using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foci_csapat_menedzser
{
    public class Player
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public List<string> Nationalities { get; set; }
        public int MarketValue { get; set; }
        public bool IsAvailable { get; set; }
        public string UnavailableReason { get; set; }
        public DateTime? ReturnDate { get; set; }
        public int JerseyNumber { get; set; }
        public int Height { get; set; }
        public string PreferredFoot { get; set; }
        public string Position { get; set; }
        public DateTime JoinedTeam { get; set; }
        public DateTime ContractEnd { get; set; }

        public int Age
        {
            get
            {
                int age = DateTime.Now.Year - BirthDate.Year;
                if (DateTime.Now.DayOfYear < BirthDate.DayOfYear)
                    age--;
                return age;
            }
        }

        public string NationalitiesDisplay
        {
            get
            {
                if (Nationalities == null || Nationalities.Count == 0)
                    return "Nincs megadva";
                if (Nationalities.Count == 1)
                    return Nationalities[0];
                return $"{Nationalities[0]} + {Nationalities.Count - 1}";
            }
        }
    }
}