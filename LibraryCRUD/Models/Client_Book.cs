using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryCRUD.Models
{
    public class Client_Book
    {
        [Key]
        public int Id { get; set; }

        public virtual Client Client { get; set; }
        public virtual Book Book { get; set; }

    }
}
