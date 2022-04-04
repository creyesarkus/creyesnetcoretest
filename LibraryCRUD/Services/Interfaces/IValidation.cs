using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryCRUD.Services.Interfaces
{
    public interface IValidation
    {
        bool IsDuplicate(string value, List<string> records);
    }
}
