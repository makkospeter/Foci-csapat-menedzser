using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foci_csapat_menedzser
{
    public class Availability
    {
        public bool IsAvailable { get; }
        public string Reason { get; }

        public Availability(bool isAvailable)
        {
            IsAvailable = isAvailable;
            Reason = null;
        }

        public Availability(string reason)
        {
            IsAvailable = false;
            Reason = reason;
        }

        public override string ToString()
        {
            return IsAvailable ? "Elérhető" : $"Nem elérhető: {Reason}";
        }
    }
}
