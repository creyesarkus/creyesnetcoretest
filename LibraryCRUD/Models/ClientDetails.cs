using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LibraryCRUD.Models
{
    public class ClientDetails
    {
        public Client Client { get; set; }
        public List<Client_Book> Client_Books { get; set; }
        public List<Book> Books { get; set; }

        public int ClientId { get; set; }
        
        [Required(ErrorMessage = "Option required")]
        public int BookId { get; set; }

    }
}
