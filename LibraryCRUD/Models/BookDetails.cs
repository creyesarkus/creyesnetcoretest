using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryCRUD.Models
{
    public class BookDetails
    {
        public Book Book { get; set; }
        public List<Author_Book> Book_Authors { get; set; }
        public List<Author> Authors{ get; set; }

        public int BookId { get; set; }

        [Required(ErrorMessage = "Option is required")]
        public int AuthorId { get; set; }

    }
}
