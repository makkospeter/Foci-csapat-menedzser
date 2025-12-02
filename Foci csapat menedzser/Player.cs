using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Foci_csapat_menedzser
{
    public class Player
    {
        public static Dictionary<string, string> countryCodes;

        static Player()
        {
            LoadNationalitiesFromJson();
        }
        private static void LoadNationalitiesFromJson()
        {
            const string jsonPath = "nationalities.json";

            try
            {
                if (File.Exists(jsonPath))
                {
                    string json = File.ReadAllText(jsonPath);
                    var jsonString = JsonSerializer.Deserialize<Dictionary<string, string>>(json);

                    if (jsonString == null)
                    {
                        countryCodes = new Dictionary<string, string>();
                    }
                    else
                    {
                        countryCodes = jsonString;
                    }
                }
                else
                {
                    countryCodes = new Dictionary<string, string>();
                }
            }
            catch
            {
                countryCodes = new Dictionary<string, string>();
            }
        }

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
                {
                    age--;
                }

                return age;
            }
        }

        public string FirstNationality
        {
            get
            {
                if (Nationalities == null || Nationalities.Count == 0)
                {
                    if (Nationalities == null)
                    {
                        Nationalities = new List<string>();
                    }

                    Nationalities.Add("Hontalan");

                    return "Hontalan";
                }

                return Nationalities[0];
            }
        }

        public string FirstNationalityCode
        {
            get
            {
                return GetCountryCode(FirstNationality);
            }
        }

        public string NationalitiesDisplay
        {
            get
            {
                if (Nationalities == null || Nationalities.Count == 0)
                    return "Nincs nemzetiség";

                return string.Join(", ", Nationalities);
            }
        }

        private string GetCountryCode(string nationality)
        {
            foreach (var codes in countryCodes)
            {
                if (codes.Value.Equals(nationality, StringComparison.OrdinalIgnoreCase))
                {
                    return codes.Key;
                }
            }

            return "unknown";
        }
    }
}