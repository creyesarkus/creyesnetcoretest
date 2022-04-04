using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryCRUD.Models
{
    public class Author_Book
    {
        [Key]
        public int Id { get; set; }

        public virtual Author Author { get; set; }
        public virtual Book Book { get; set; }
    }
}
