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
        private List<string> Nationalities {  get; set; }
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
    }
}
