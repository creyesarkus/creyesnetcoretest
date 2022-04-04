using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryCRUD.Models
{
    public class AuthorDetails
    {
        public Author Author { get; set; }
        public List<Author_Book> Author_Books { get; set; }
        public List<Book> Books { get; set; }

        public int AuthorId { get; set; }

        [Required(ErrorMessage = "Option is required")]
        public int BookId { get; set; }
    }
}
