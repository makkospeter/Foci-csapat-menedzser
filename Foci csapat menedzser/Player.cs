using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foci_csapat_menedzser
{
    public class Player
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public string BirthDate { get; set; } = string.Empty;
        public string Nationality { get; set; } = string.Empty;
        public int MarketValue { get; set; }
        public bool IsAvailable { get; set; }
    }
}