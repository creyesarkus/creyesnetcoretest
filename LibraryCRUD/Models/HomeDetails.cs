using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryCRUD.Models
{
    public class HomeDetails
    {
        public int Authors { get; set; }
        public int Books { get; set; }

        public int LendedBooks { get; set; }
        public int Clients { get; set; }
    }
}
