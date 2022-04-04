using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryCRUD.Services.Interfaces;

namespace LibraryCRUD.Services
{
    public class Validation : IValidation
    {
        public bool IsDuplicate(string value, List<string> records)
        {
            foreach(string record in records)
            {
                if(value == record)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
