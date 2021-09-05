using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema
{
    public class Seats
    {
        public List<Sit> seats { get; set; }
    }

    public class Sit
    {
        public int Number { get; set; }
        public bool IsFull { get; set; }
    }
}
